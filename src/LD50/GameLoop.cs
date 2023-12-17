using LD50.Utilities;
using System;
using System.Diagnostics;

namespace LD50 {
    public class GameLoop(
        IGameEvents gameEvents,
        UpdateInfo updateInfo,
        SlowUpdateMarker slowUpdateMarker,
        IInitializable initializable,
        IFixedUpdateable fixedUpdateable,
        IVariableUpdateable variableUpdateable,
        IDrawable drawable)
        : IStartupHandler {

        private static readonly TimeSpan _maxFrameTime = TimeSpan.FromSeconds(0.25d);
        private static readonly TimeSpan _fixedDeltaTime = TimeSpan.FromSeconds(1d / 60d);

        private readonly Stopwatch _stopwatch = new();

        private TimeSpan _currentUpdateTime;
        private TimeSpan _currentDrawTime;
        private TimeSpan _totalTime;
        private TimeSpan _totalDrawTime;
        private TimeSpan _accumulator;

        public void OnStartup() {
            gameEvents.Initialized += OnInitialized;
            gameEvents.Updated += OnUpdated;
            gameEvents.Drawn += OnDrawn;
        }

        private void OnInitialized(object? sender, EventArgs e) {
            initializable.Initialize();

            _stopwatch.Start();
        }

        private void OnUpdated(object? sender, EventArgs e) {
            updateInfo.UpdateDeltaTime(_fixedDeltaTime);
            updateInfo.UpdateBlendFactor(1f); // Doesn't make sense to blend during update.

            TimeSpan newTime = _stopwatch.Elapsed;
            TimeSpan frameTime = (newTime - _currentUpdateTime) * GameProperties.Speed;
            if (frameTime > _maxFrameTime) {
                frameTime = _maxFrameTime;
            }
            _currentUpdateTime = newTime;

            _accumulator += frameTime;

            if (slowUpdateMarker.WasSlowUpdate) {
                _accumulator = _fixedDeltaTime;
                slowUpdateMarker.Clear();
            }

            while (_accumulator >= _fixedDeltaTime) {
                fixedUpdateable.FixedUpdate();

                _totalTime += _fixedDeltaTime;
                updateInfo.UpdateTotalTime(_totalTime);

                _accumulator -= _fixedDeltaTime;

                if (slowUpdateMarker.WasSlowUpdate) {
                    return;
                }
            }

            float blendFactor = (float)(_accumulator / _fixedDeltaTime);
            updateInfo.UpdateBlendFactor(blendFactor);
        }

        private void OnDrawn(object? sender, EventArgs e) {
            if (slowUpdateMarker.WasSlowUpdate) {
                return;
            }

            TimeSpan newTime = _stopwatch.Elapsed;
            TimeSpan frameTime = (newTime - _currentDrawTime) * GameProperties.Speed;
            _currentDrawTime = newTime;

            _totalDrawTime += frameTime;

            updateInfo.UpdateDeltaTime(frameTime);
            updateInfo.UpdateTotalTime(_totalDrawTime);

            variableUpdateable.VariableUpdate();
            drawable.Draw();
        }
    }
}
