using Microsoft.Xna.Framework;

namespace LD50.Graphics {
    public class GraphicsDeviceManagerInitializer(GraphicsDeviceManager graphicsDeviceManager)
        : IInitializable {
        
        public void Initialize() {
            graphicsDeviceManager.PreferredBackBufferWidth = GameProperties.ScreenWidth;
            graphicsDeviceManager.PreferredBackBufferHeight = GameProperties.ScreenHeight;
            graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            graphicsDeviceManager.ApplyChanges();
        }
    }
}
