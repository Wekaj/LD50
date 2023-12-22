using System;

namespace LD50.Screens {
    public class ScreenChanger {
        public event EventHandler<ScreenArgs>? Transitioned;

        public void ChangeScreen(ScreenArgs args) {
            Transitioned?.Invoke(this, args);
        }
    }
}
