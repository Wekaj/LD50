using Microsoft.Xna.Framework;

namespace LD50.Screens {
    public interface IScreen {
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}
