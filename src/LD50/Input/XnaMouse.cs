using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LD50.Input {
    public class XnaMouse {
        private readonly GameWindow _window;

        public Vector2 Position { get; private set; }

        public XnaMouse(GameWindow window) {
            _window = window;
        }

        public void Update() {
            MouseState mouseState = Mouse.GetState(_window);

            Position = mouseState.Position.ToVector2();
        }
    }
}
