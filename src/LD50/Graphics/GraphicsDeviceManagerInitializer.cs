using LD50.Development;
using Microsoft.Xna.Framework;

namespace LD50.Graphics {
    public class GraphicsDeviceManagerInitializer(
        Game game,
        EngineEnvironment engineEnvironment,
        GraphicsDeviceManager graphicsDeviceManager)
        : IInitializable {
        
        public void Initialize() {
            if (engineEnvironment.ProjectDirectory is not null) {
                game.Window.Title = $"{GameProperties.Name} (Development Mode)";
            }
            else {
                game.Window.Title = GameProperties.Name;
            }

            graphicsDeviceManager.PreferredBackBufferWidth = GameProperties.ScreenWidth;
            graphicsDeviceManager.PreferredBackBufferHeight = GameProperties.ScreenHeight;
            graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            graphicsDeviceManager.ApplyChanges();
        }
    }
}
