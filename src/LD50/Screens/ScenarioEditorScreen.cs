using LD50.Interface;
using LD50.Levels;
using Microsoft.Xna.Framework;

namespace LD50.Screens {
    public class ScenarioEditorScreen(
        World world,
        ScreenChanger screenChanger)
        : IScreen {
        
        public void Show(ScreenArgs args) {
            world.Elements.Add(new Element {
                Position = new Vector2(GameProperties.ScreenWidth - 200f - 8f, 8f),
                Size = new Vector2(200f, 20f),
                Label = "Home",
                OnClick = () => screenChanger.ChangeScreen(new ScreenArgs(ScreenType.Engine)),
            });

            world.Elements.Add(new Element {
                Position = new Vector2(GameProperties.ScreenWidth * 0.5f - 100f, 8f),
                Size = new Vector2(200f, 20f),
                Label = "Scenario Editor",
            });
        }

        public void Hide() {
            world.Reset();
        }
    }
}
