using Microsoft.Xna.Framework;

namespace LD50.Graphics {
    public class GraphicsDeviceManagerInitializer(GraphicsDeviceManager graphicsDeviceManager)
        : IInitializable {
        
        public void Initialize() {
            graphicsDeviceManager.PreferredBackBufferWidth = 960;
            graphicsDeviceManager.PreferredBackBufferHeight = 600;
            graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            graphicsDeviceManager.ApplyChanges();
        }
    }
}
