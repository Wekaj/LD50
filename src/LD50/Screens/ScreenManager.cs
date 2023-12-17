using LD50.Utilities;

namespace LD50.Screens {
    public class ScreenManager : IFixedUpdateable, IDrawable {
        private readonly GameScreen _gameScreen;
        private readonly IGameTimeSource _gameTimeSource;

        private IScreen _currentScreen;

        public ScreenManager(GameScreen gameScreen, IGameTimeSource gameTimeSource) {
            _gameScreen = gameScreen;
            _gameTimeSource = gameTimeSource;

            _currentScreen = _gameScreen;
        }

        public void FixedUpdate() {
            _currentScreen.Update(_gameTimeSource.Latest);
        }

        public void Draw() {
            _currentScreen.Draw(_gameTimeSource.Latest);
        }
    }
}
