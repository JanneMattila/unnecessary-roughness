using System;
using UR.Animation;
using UR.Data;

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

        public Action<ObjectAnimation[], ObjectAnimation> ExecuteAnimations;
        public Action<Game> ExecuteDraw;
        public Action<string, bool> ShowElement;
        public Action<string> HideElement;

        public GameEngine(IBrowserLogger logger, Game game, IRandomizer randomizer, IEventStore eventStore)
        {
            _logger = logger;
            _randomizer = randomizer;
            _eventStore = eventStore;
            _game = game;
        }
    }
}
