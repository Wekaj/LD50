using LD50.Utilities;

namespace LD50.Screens {
    public class ScreenManager(
        GameScreen gameScreen,
        IGameTimeSource gameTimeSource)
        : IInitializable, IFixedUpdateable, IDrawable {

        private IScreen? _currentScreen;

        public void Initialize() {
            ShowScreen(gameScreen);
        }

        public void FixedUpdate() {
            _currentScreen?.Update(gameTimeSource.Latest);
        }

        public void Draw() {
            _currentScreen?.Draw(gameTimeSource.Latest);
        }

        private void ShowScreen(IScreen screen) {
            _currentScreen?.Hide();
            _currentScreen = screen;
            _currentScreen.Show();
        }
    }
}
