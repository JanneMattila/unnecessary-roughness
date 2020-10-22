﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using UR.Data;

namespace UR.Client.Pages
{
    public class PlayBase : ComponentBase, IDisposable
    {
        private bool disposedValue;

        [Parameter]
        public string GameID { get; set; }

        [Inject] private HttpClient Http { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }

        protected static GameEngine _gameEngine;
        protected static Action _stateHasChanged;
        protected static IJSRuntime JSRuntime2;
        protected bool _loaded = false;
        protected static ElementReference _canvas;

        protected override async Task OnInitializedAsync()
        {
            // TODO: Fix static identifiers.
            JSRuntime2 = JSRuntime;

            _stateHasChanged = () => StateHasChanged();

            await base.OnInitializedAsync();

            if (_gameEngine == null)
            {
                var randomizer = new RandomizerBackend
                {
                    FetchDicesAsync = async (id, type, count) => await Http.GetStringAsync($"api/Randomizer/Dices/{id}/{type}/{count}")
                };

                var eventStore = new EventStoreBackend
                {
                    HttpPutEventAsync = async (id, e) => await Http.PutAsync($"api/Events/{id}", new StringContent(e, System.Text.Encoding.Unicode, "application/xml")),
                    HttpGetEventsAsync = async (id) => await Http.GetStringAsync($"api/Events/{id}")
                };

                _gameEngine = new GameEngine(new BrowserLogger(), new Data.Game()
                {
                    ID = "123",
                    HomeTeam = new Team(),
                    VisitorTeam = new Team()
                }, randomizer, eventStore)
                {
                    ExecuteAnimations = (playerAnimations, ballAnimation) =>
                    {
                        JSRuntime.InvokeAsync<object>("animate", playerAnimations, ballAnimation);
                    },

                    ExecuteDraw = (game) =>
                    {
                        JSRuntime.InvokeAsync<object>("drawCanvas", game);
                        _stateHasChanged();
                    },

                    ShowElement = (id, modal) =>
                    {
                        JSRuntime.InvokeAsync<object>("showElement", id, modal);
                        _stateHasChanged();
                    },

                    HideElement = (id) =>
                    {
                        JSRuntime.InvokeAsync<object>("hideElement", id);
                        _stateHasChanged();
                    }
                };
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!_loaded)
            {
                _loaded = true;
                await _gameEngine.LoadGameEventsAsync();

                await JSRuntime.InvokeAsync<object>("initializeGameview", _canvas);
                await JSRuntime.InvokeAsync<object>("drawCanvas", _gameEngine.Game);

                _stateHasChanged();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects)
                    _gameEngine = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        [JSInvokable]
        public static async void CanvasClickReceived(int x, int y)
        {
            await Task.CompletedTask;
            //await _gameEngine.CanvasClickAsync(x, y);
            _stateHasChanged();
        }

        [JSInvokable]
        public static async void CanvasKeyUpReceived(int keyCode)
        {
            await Task.CompletedTask;
            //_gameStateEngine.CanvasKeyPress(keyCode);
            //await JSRuntime2.InvokeAsync<object>("drawCanvas", _gameStateEngine.Game);
            //_debug = $"{keyCode}";
            _stateHasChanged();
        }
    }
}
