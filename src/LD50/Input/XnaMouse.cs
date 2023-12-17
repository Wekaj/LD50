using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LD50.Input {
    public class XnaMouse(Game game)
        : IFixedUpdateable {
        
        public Vector2 Position { get; private set; }

        public void FixedUpdate() {
            MouseState mouseState = Mouse.GetState(game.Window);

            Position = mouseState.Position.ToVector2();
        }
    }
}
