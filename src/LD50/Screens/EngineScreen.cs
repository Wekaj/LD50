using LD50.Interface;
using LD50.Levels;
using Microsoft.Xna.Framework;

namespace LD50.Screens {
    public class EngineScreen(World world)
        : IScreen {

        public void Show() {
            world.Elements.Add(new Element {
                Position = new Vector2(GameProperties.ScreenWidth * 0.5f - 100f, 8f),
                Size = new Vector2(200f, 20f),
                Label = $"{GameProperties.Name} Engine",
            });
        }

        public void Hide() {
        }

        public void Update(GameTime gameTime) {
        }

        public void Draw(GameTime gameTime) {
        }
    }
}
