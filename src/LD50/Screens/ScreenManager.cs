using Microsoft.Xna.Framework;

namespace LD50.Screens {
    public class ScreenManager {
        private readonly GameScreen _gameScreen;

        private IScreen _currentScreen;

        public ScreenManager(GameScreen gameScreen) {
            _gameScreen = gameScreen;

            _currentScreen = _gameScreen;
        }

        public void Update(GameTime gameTime) {
            _currentScreen.Update(gameTime);
        }

        public void Draw(GameTime gameTime) {
            _currentScreen.Draw(gameTime);
        }
    }
}
