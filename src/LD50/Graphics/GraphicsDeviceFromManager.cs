using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD50.Graphics {
    public class GraphicsDeviceFromManager(GraphicsDeviceManager graphicsDeviceManager)
        : IGraphicsDeviceSource {

        public GraphicsDevice GraphicsDevice => graphicsDeviceManager.GraphicsDevice;
    }
}
