using Microsoft.Xna.Framework;

namespace LD50.Utilities {
    public interface IGameTimeSource {
        GameTime Latest { get; }
    }
}
