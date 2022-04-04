using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD50.Entities {
    public class Entity {
        public Vector2 Position { get; set; }

        public Texture2D? Texture { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public SpriteEffects Effects { get; set; }

        public Vector2? TargetPosition { get; set; }
        public float HopTimer { get; set; }
    }
}
