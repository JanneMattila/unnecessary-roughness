using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UR.Animation;
using UR.Data;
using UR.Events;

namespace UR
{
    public class GameEngine
    {
        private readonly IBrowserLogger _logger;
        private readonly IRandomizer _randomizer;
        private readonly IEventStore _eventStore;
        private readonly Player[] _boardPositions = new Player[GameBoard.BoardWidth * GameBoard.BoardHeight];

        private GameState _gameState = GameState.Initialization;
        private Game _game;

        public Game Game => _game;

        public Action<ObjectAnimation[], ObjectAnimation> ExecuteAnimations;
        public Action<Game> ExecuteDraw;
        public Action<string, bool> ShowElement;
        public Action<string> HideElement;
        
        internal int ShortestPathNodeScanCount { get; set; }

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

            //if (createGameEvent)
            //{
            //    await _eventStore.AppendEventAsync(_game.ID, events.First());
            //}

            ExecuteDraw(_game);
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
            await Task.CompletedTask;
        }

        private void ShortestPath(ShortestPathNode scanPosition, int[] board, List<ShortestPathNode> previousPositions, List<ShortestPathNode> newPositions, int x, int y, int movement, List<BoardPosition> availableMoves, List<BoardPosition> availableExtraMoves)
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
}
