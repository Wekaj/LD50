using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace LD50 {
    public class LD50Game : Game, IGameEvents {
        public LD50Game() {
            IsMouseVisible = true;
            IsFixedTimeStep = false;
        }

        public event EventHandler? Initialized;
        public event EventHandler? Updated;
        public event EventHandler? Drawn;

        protected override void Initialize() {
            base.Initialize();

            Initialized?.Invoke(this, EventArgs.Empty);
        }

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);

            Updated?.Invoke(this, EventArgs.Empty);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.DimGray);

            base.Draw(gameTime);

            Drawn?.Invoke(this, EventArgs.Empty);
        }
    }
}
