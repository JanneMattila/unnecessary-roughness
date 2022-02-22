using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using UR.Animation;
using UR.Data;
using UR.Events;

namespace UR;

public class GameEngine
{
    private readonly IBrowserLogger _logger;
    private readonly IRandomizer _randomizer;
    private readonly IEventStore _eventStore;
    private readonly Player[] _boardPositions = new Player[GameBoard.BoardWidth * GameBoard.BoardHeight];

    private GameState _gameState = GameState.Initialization;
    private Game _game;

    private string _currentTeam = string.Empty;
    private bool _isAnimationEnabled = false;

    public Team CurrentTeam { get => _currentTeam == _game.HomeTeam.ID ? _game.HomeTeam : _game.VisitorTeam; }

    public Game Game => _game;

    [AllowNull]
    public Action<Game> ExecuteDraw;
    private void Draw() => ExecuteDraw(_game);

    [AllowNull]
    public Action<ObjectAnimation[], ObjectAnimation> ExecuteAnimations;

    [AllowNull]
    public Action<string, bool> ShowElement;

    [AllowNull]
    public Action<string> HideElement;

    internal int FloodFillNodeScanCount { get; set; }
    internal int FloodFillNormalMovesCount { get; set; }
    internal int FloodFillExtraMovesCount { get; set; }
    internal int ShortestPathNodeScanCount { get; set; }

    public string ActionMenuVisibility = ElementVisibility.VisibilityNoneElement;

    private bool _playerInformationVisibility;
    public bool PlayerInformationVisibility
    {
        get
        {
            return _playerInformationVisibility;
        }
        set
        {
            _playerInformationVisibility = value;
            if (!_isAnimationEnabled)
            {
                if (value)
                {
                    ShowElement(nameof(PlayerInformationVisibility), false);
                }
                else
                {
                    HideElement(nameof(PlayerInformationVisibility));
                }
            }
        }
    }

    private bool _placingPlayersVisibility;
    public bool PlacingPlayersVisibility
    {
        get
        {
            return _placingPlayersVisibility;
        }
        set
        {
            _placingPlayersVisibility = value;
            if (!_isAnimationEnabled)
            {
                if (value)
                {
                    ShowElement(nameof(PlacingPlayersVisibility), false);
                }
                else
                {
                    HideElement(nameof(PlacingPlayersVisibility));
                }
            }
        }
    }

    private bool _endTurnVisibility;
    public bool EndTurnVisibility
    {
        get
        {
            return _endTurnVisibility;
        }
        set
        {
            _endTurnVisibility = value;
            if (!_isAnimationEnabled)
            {
                if (value)
                {
                    ShowElement(nameof(EndTurnVisibility), false);
                }
                else
                {
                    HideElement(nameof(EndTurnVisibility));
                }
            }
        }
    }
    public GameEngine(IBrowserLogger logger, Game game, IRandomizer randomizer, IEventStore eventStore)
    {
        _logger = logger;
        _randomizer = randomizer;
        _eventStore = eventStore;
        _game = game;
    }

    public async Task LoadGameEventsAsync()
    {
        _gameState = GameState.Initialization;
        var events = new List<Event>();

        try
        {
            events = await _eventStore.GetEventsAsync(_game.ID);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        var createGameEvent = false;
        if (!events.Any())
        {
            // Create initial data entry about the game data content.
            var evt = new GameDataEvent()
            {
                Game = _game
            };

            events.Add(evt);
            createGameEvent = true;
        }

        await SetEventsAsync(events);

        if (createGameEvent)
        {
            await _eventStore.AppendEventAsync(_game.ID, events.First());
        }

        UpdateBoardPositions();
        Draw();
    }

    public async Task SetEventsAsync(List<Event> events)
    {
        foreach (var e in events)
        {
            await ExecuteEventAsync(e);
        }
    }

    public async Task ExecuteEventAsync(Event e)
    {
        Console.Write(e);
        if (e is GameDataEvent gameDataEvent)
        {
            _currentTeam = gameDataEvent.Game.HomeTeam.ID;
            _gameState = GameState.VisitorPlacingPlayers;
            PlacingPlayersVisibility = ElementVisibility.VisibilityNormal;
        }
        else if (e is PlacePlayersEvent)
        {
            if (_gameState == GameState.VisitorPlacingPlayers)
            {
                _gameState = GameState.HomePlacingPlayers;
                _currentTeam = _game.HomeTeam.ID;
            }
            else if (_gameState == GameState.HomePlacingPlayers)
            {
                _gameState = GameState.Normal;
                _currentTeam = _game.HomeTeam.ID;
                PlacingPlayersVisibility = ElementVisibility.VisibilityNone;
                EndTurnVisibility = ElementVisibility.VisibilityNormal;
            }
        }
        else if (e is EndTurnEvent)
        {
            SwapTeamTurn();
        }

        Draw();
        await Task.CompletedTask;
    }

    private void SwapTeamTurn()
    {
        _currentTeam = _currentTeam == _game.HomeTeam.ID ?
            _game.VisitorTeam.ID : _game.HomeTeam.ID;
    }

    public Player GetPlayerFromBoardPosition(int x, int y)
    {
        return _boardPositions[x + y * GameBoard.BoardWidth];
    }

    public async Task CanvasClickAsync(int x, int y)
    {
        _logger.Log(LogLevel.Trace, nameof(CanvasClickAsync));

        Player selectedPlayer = GetPlayerFromBoardPosition(x, y);
        _logger.Log(LogLevel.Trace, $"Player click: {selectedPlayer?.Name}");

        //FloodFillNodeScanCount = 0;
        //FloodFillNormalMovesCount = 0;
        //FloodFillExtraMovesCount = 0;

        PlayerInformationVisibility = ElementVisibility.VisibilityNone;

        if (_gameState == GameState.Kick)
        {
            ClearGameBoardSelection();

            _game.AvailableMoves.Add(new BoardPosition()
            {
                X = x,
                Y = y
            });
        }
        else if (_gameState == GameState.Normal)
        {
            await CanvasClickNormalAsync(x, y, selectedPlayer);
        }
        else if (_gameState == GameState.VisitorPlacingPlayers || _gameState == GameState.HomePlacingPlayers)
        {
            CanvasClickPlacingPlayers(x, y, selectedPlayer);
        }
    }

    public async Task PlacingPlayers()
    {
        int lineOfScrimmageY;
        if (_currentTeam == _game.HomeTeam.ID)
        {
            lineOfScrimmageY = GameBoard.BoardBottomHalf;
        }
        else
        {
            lineOfScrimmageY = GameBoard.BoardTopHalf;
        }

        var team = CurrentTeam;
        if (ValidatePlacingPlayers(team, lineOfScrimmageY))
        {
            var placePlayersEvent = new PlacePlayersEvent()
            {
                ID = Guid.NewGuid(),
                InitiatedBy = team.Coach.ID,
                Players = team.Players.ToList()
            };

            await ExecuteEventAsync(placePlayersEvent);
            await _eventStore.AppendEventAsync(_game.ID, placePlayersEvent);
        }
    }

    public async Task EndTurn()
    {
        var endTurnEvent = new EndTurnEvent()
        {
            ID = Guid.NewGuid(),
            InitiatedBy = CurrentTeam.Coach.ID
        };
        await ExecuteEventAsync(endTurnEvent);
        await _eventStore.AppendEventAsync(_game.ID, endTurnEvent);
    }

    internal bool ValidatePlacingPlayers(Team currentTeam, int lineOfScrimmageY)
    {
        return true;
    }

    private void UpdateBoardPositions()
    {
        var size = GameBoard.BoardWidth * GameBoard.BoardHeight;
        for (var i = 0; i < size; i++)
        {
            _boardPositions[i] = Player.Empty;
        }

        var allPlayers = _game.HomeTeam.Players.ToList();
        allPlayers.AddRange(_game.VisitorTeam.Players);
        foreach (var player in allPlayers)
        {
            _boardPositions[player.BoardPosition.X + player.BoardPosition.Y * GameBoard.BoardWidth] = player;
        }
        Draw();
    }

    private void CanvasClickPlacingPlayers(int x, int y, Player selectedPlayer)
    {
        _logger.Log(LogLevel.Trace, nameof(CanvasClickPlacingPlayers));
        _logger.Log(LogLevel.Trace, $"Selected player {selectedPlayer.Name} and {_game.SelectedPlayer.Name}");

        ClearGameBoardSelection();

        if (selectedPlayer == Player.Empty && _game.SelectedPlayer != Player.Empty)
        {
            if (_currentTeam == _game.SelectedPlayer.Team)
            {
                // Can move only own players.
                if ((_currentTeam == _game.HomeTeam.ID && y <= GameBoard.BoardBottomHalf) ||
                    (_currentTeam == _game.VisitorTeam.ID && y >= GameBoard.BoardTopHalf))
                {
                    // Inside own side
                    _game.SelectedPlayer.BoardPosition.X = x;
                    _game.SelectedPlayer.BoardPosition.Y = y;
                    _game.SelectedPlayer = Player.Empty;

                    UpdateBoardPositions();
                }
                else
                {
                    // Trying to move outside own side.
                    _game.SelectedPlayer = Player.Empty;
                }
            }
            else
            {
                _game.SelectedPlayer = Player.Empty;
            }
        }
        else if (selectedPlayer != Player.Empty && _game.SelectedPlayer != selectedPlayer)
        {
            _logger.Log(LogLevel.Trace, $"Select player {selectedPlayer?.Name}");

            _game.SelectedPlayer = selectedPlayer;
            PlayerInformationVisibility = ElementVisibility.VisibilityNormal;
        }
        else
        {
            _logger.Log(LogLevel.Trace, $"Un-select player");

            _game.SelectedPlayer = Player.Empty;
        }
    }

    private async Task CanvasClickNormalAsync(int x, int y, Player selectedPlayer)
    {
        _logger.Log(LogLevel.Trace, nameof(CanvasClickNormalAsync));

        if (_game.SelectedPlayer != Player.Empty && selectedPlayer == Player.Empty)
        {
            if (_game.SelectedPlayer.Team == CurrentTeam.ID)
            {
                var targetIndex = x + y * GameBoard.BoardWidth;
                if (_game.SelectedMoves.Count > 0 &&
                    _game.SelectedMoves.Last().X == x &&
                    _game.SelectedMoves.Last().Y == y)
                {
                    // Player has selected last element of highlighted move path
                    var moves = _game.SelectedMoves.ToList();
                    ClearGameBoardSelection();

                    var playerMoveEvent = new MovePlayerEvent()
                    {
                        ID = Guid.NewGuid(),
                        PlayerID = _game.SelectedPlayer.ID,
                        InitiatedBy = CurrentTeam.Coach.ID,
                        Moves = moves
                    };

                    //Status = $"{_game.SelectedPlayer.Name} is on the run.";
                    await ExecuteEventAsync(playerMoveEvent);
                    await _eventStore.AppendEventAsync(_game.ID, playerMoveEvent);

                    _game.SelectedPlayer.MovementLeft -= moves.Count;
                    _game.SelectedPlayer.BoardPosition.X = x;
                    _game.SelectedPlayer.BoardPosition.Y = y;

                    ShowAllAvailableMovesOfPlayer(_game.SelectedPlayer);
                }
                else
                {
                    // Lets calculate shortest path to get to the target
                    var allAvailableMoves = _game.AvailableMoves.ToList();
                    allAvailableMoves.AddRange(_game.AvailableExtraMoves);

                    if (allAvailableMoves.Any(m => m.X == x && m.Y == y))
                    {
                        // Selection within available moves area
                        _game.SelectedMoves.Clear();

                        var allPlayers = _game.HomeTeam.Players.ToList();
                        allPlayers.AddRange(_game.VisitorTeam.Players);
                        allPlayers.Remove(_game.SelectedPlayer);

                        var playerX = _game.SelectedPlayer.BoardPosition.X;
                        var playerY = _game.SelectedPlayer.BoardPosition.Y;

                        var availableMoves = new List<BoardPosition>();
                        var availableExtraMoves = new List<BoardPosition>();

                        var newPositions = new List<ShortestPathNode>();
                        var startPosition = new BoardPosition() { X = playerX, Y = playerY };
                        var previousPositions = new List<ShortestPathNode>
                        {
                            new ShortestPathNode()
                            {
                                Position = startPosition,
                                Path = new List<BoardPosition>() { startPosition }
                            }
                        };

                        var board = new int[GameBoard.BoardWidth * GameBoard.BoardHeight];
                        const int max = 99999;
                        for (var i = 0; i < _boardPositions.Length; i++)
                        {
                            board[i] = _boardPositions[i] != Player.Empty ? -1 : max;
                        }

                        for (var i = 0; i < _game.SelectedPlayer.MovementLeft; i++)
                        {
                            ShortestPath(null, board, previousPositions, newPositions, playerX, playerY, _game.SelectedPlayer.MovementLeft + 2 - i + 1, availableMoves, availableExtraMoves);
                            previousPositions = newPositions;
                            newPositions = new List<ShortestPathNode>();

                            if (board[targetIndex] != max)
                            {
                                // We have found shortest path so we can stop.
                                var shortestPath = previousPositions.First(p => p.Position.X == x && p.Position.Y == y);
                                _game.SelectedMoves.AddRange(shortestPath.Path);
                                break;
                            }
                        }
                    }
                    else
                    {
                        // Selection outside the range of available moves area
                        ClearGameBoardSelection();
                    }
                }
            }
            Draw();
        }
        else if (selectedPlayer == _game.SelectedPlayer || selectedPlayer == Player.Empty)
        {
            // Clear all selections
            ClearGameBoardSelection();
            _game.SelectedPlayer = Player.Empty;
        }
        else
        {
            ShowAllAvailableMovesOfPlayer(selectedPlayer);
        }
        await Task.CompletedTask;
    }

    private void ShowAllAvailableMovesOfPlayer(Player player)
    {
        // Show all the available moves for selected player
        ClearGameBoardSelection();

        _game.SelectedPlayer = player;

        var allPlayers = _game.HomeTeam.Players.ToList();
        allPlayers.AddRange(_game.VisitorTeam.Players);
        allPlayers.Remove(player);

        var playerX = player.BoardPosition.X;
        var playerY = player.BoardPosition.Y;

        var availableMoves = new List<BoardPosition>();
        var availableExtraMoves = new List<BoardPosition>();

        var newPositions = new List<BoardPosition>();
        var previousPositions = new List<BoardPosition>
            {
                new BoardPosition() { X = playerX, Y = playerY }
            };

        var board = new int[GameBoard.BoardWidth * GameBoard.BoardHeight];
        for (var i = 0; i < _boardPositions.Length; i++)
        {
            board[i] = _boardPositions[i] == Player.Empty ? 0 : 1;
        }

        for (var i = 0; i < player.MovementLeft + 2; i++)
        {
            FloodFill(true, board, previousPositions, newPositions, playerX, playerY, player.MovementLeft + 2 - i + 1, availableMoves, availableExtraMoves);
            previousPositions = newPositions;
            newPositions = new List<BoardPosition>();
        }

        _game.AvailableMoves.AddRange(availableMoves);

        PlayerInformationVisibility = ElementVisibility.VisibilityNormal;
        ActionMenuVisibility = ElementVisibility.VisibilityNoneElement;

        Draw();
    }

    private void ClearGameBoardSelection()
    {
        _logger.Log(LogLevel.Trace, nameof(ClearGameBoardSelection));

        _game.AvailableMoves.Clear();
        _game.SelectedMoves.Clear();
    }

    private void FloodFill(bool scan, int[] board, List<BoardPosition> previousPositions, List<BoardPosition> newPositions, int x, int y, int movement, List<BoardPosition> availableMoves, List<BoardPosition> availableExtraMoves)
    {
        if (scan)
        {
            foreach (var existingPosition in previousPositions)
            {
                FloodFill(false, board, previousPositions, newPositions, existingPosition.X - 1, existingPosition.Y - 1, movement - 1, availableMoves, availableExtraMoves);
                FloodFill(false, board, previousPositions, newPositions, existingPosition.X - 1, existingPosition.Y + 1, movement - 1, availableMoves, availableExtraMoves);
                FloodFill(false, board, previousPositions, newPositions, existingPosition.X + 1, existingPosition.Y + 1, movement - 1, availableMoves, availableExtraMoves);
                FloodFill(false, board, previousPositions, newPositions, existingPosition.X + 1, existingPosition.Y - 1, movement - 1, availableMoves, availableExtraMoves);
                FloodFill(false, board, previousPositions, newPositions, existingPosition.X - 1, existingPosition.Y, movement - 1, availableMoves, availableExtraMoves);
                FloodFill(false, board, previousPositions, newPositions, existingPosition.X + 1, existingPosition.Y, movement - 1, availableMoves, availableExtraMoves);
                FloodFill(false, board, previousPositions, newPositions, existingPosition.X, existingPosition.Y + 1, movement - 1, availableMoves, availableExtraMoves);
                FloodFill(false, board, previousPositions, newPositions, existingPosition.X - 1, existingPosition.Y + 1, movement - 1, availableMoves, availableExtraMoves);
                FloodFill(false, board, previousPositions, newPositions, existingPosition.X, existingPosition.Y - 1, movement - 1, availableMoves, availableExtraMoves);
            }

            return;
        }

        var index = x + y * GameBoard.BoardWidth;
        if (movement <= 0 || x < 0 || x >= GameBoard.BoardWidth || y < 0 || y >= GameBoard.BoardHeight ||
            board[index] != 0)
        {
            // Outside board or already occupied or this move has already been placed.
            return;
        }

        FloodFillNodeScanCount++;
        board[index]++;
        newPositions.Add(new BoardPosition() { X = x, Y = y });

        if (movement > 2)
        {
            availableMoves.Add(new BoardPosition() { X = x, Y = y });
            FloodFillNormalMovesCount++;
        }
        else
        {
            availableExtraMoves.Add(new BoardPosition() { X = x, Y = y });
            FloodFillExtraMovesCount++;
        }
    }

    private void ShortestPath(ShortestPathNode? scanPosition, int[] board, List<ShortestPathNode> previousPositions, List<ShortestPathNode> newPositions, int x, int y, int movement, List<BoardPosition> availableMoves, List<BoardPosition> availableExtraMoves)
    {
        if (scanPosition == null)
        {
            foreach (var existingPosition in previousPositions)
            {
                var p = existingPosition.Position;
                ShortestPath(existingPosition, board, previousPositions, newPositions, p.X - 1, p.Y, movement - 1, availableMoves, availableExtraMoves);
                ShortestPath(existingPosition, board, previousPositions, newPositions, p.X + 1, p.Y, movement - 1, availableMoves, availableExtraMoves);
                ShortestPath(existingPosition, board, previousPositions, newPositions, p.X, p.Y + 1, movement - 1, availableMoves, availableExtraMoves);
                ShortestPath(existingPosition, board, previousPositions, newPositions, p.X - 1, p.Y + 1, movement - 1, availableMoves, availableExtraMoves);
                ShortestPath(existingPosition, board, previousPositions, newPositions, p.X, p.Y - 1, movement - 1, availableMoves, availableExtraMoves);
                ShortestPath(existingPosition, board, previousPositions, newPositions, p.X - 1, p.Y - 1, movement - 1, availableMoves, availableExtraMoves);
                ShortestPath(existingPosition, board, previousPositions, newPositions, p.X - 1, p.Y + 1, movement - 1, availableMoves, availableExtraMoves);
                ShortestPath(existingPosition, board, previousPositions, newPositions, p.X + 1, p.Y + 1, movement - 1, availableMoves, availableExtraMoves);
                ShortestPath(existingPosition, board, previousPositions, newPositions, p.X + 1, p.Y - 1, movement - 1, availableMoves, availableExtraMoves);
            }

            return;
        }

        var index = x + y * GameBoard.BoardWidth;
        var distance = scanPosition.Path.Count + 1;
        if (movement <= 0 || x < 0 || x >= GameBoard.BoardWidth || y < 0 || y >= GameBoard.BoardHeight ||
            board[index] == -1 || board[index] < distance)
        {
            // Outside board or already occupied or this move has already been placed.
            return;
        }

        ShortestPathNodeScanCount++;
        board[index] = distance;

        var position = new BoardPosition() { X = x, Y = y };
        var path = scanPosition.Path.ToList();
        path.Add(position);
        newPositions.Add(new ShortestPathNode()
        {
            Position = position,
            Path = path
        });
    }
}
