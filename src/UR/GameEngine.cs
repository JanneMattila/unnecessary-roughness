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

            var events = await _eventStore.GetEventsAsync(_game.ID);
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
    }
}
