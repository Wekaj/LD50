using System.Collections.Generic;

namespace LD50.Screens {
    public class ScreenManager(
        RunArguments runArguments,
        IDictionary<ScreenType, IScreen> screens,
        ScreenChanger screenChanger)
        : IStartupHandler, IInitializable {

        private IScreen? _currentScreen;

        public void OnStartup() {
            screenChanger.Transitioned += OnTransitioned;
        }

        public void Initialize() {
            if (runArguments.ProjectDirectory is not null) {
                ChangeScreen(new ScreenArgs(ScreenType.Engine));
            }
            else {
                ChangeScreen(new ScreenArgs(ScreenType.Game));
            }
        }

        public void ChangeScreen(ScreenArgs args) {
            _currentScreen?.Hide();
            _currentScreen = screens[args.ScreenType];
            _currentScreen.Show(args);
        }

        private void OnTransitioned(object? sender, ScreenArgs e) {
            ChangeScreen(e);
        }
    }
}
