using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD50.Entities {
    public record Entity {
        public Vector2 Position { get; set; }
        public float Depth { get; set; }
        public Vector2 Velocity { get; set; }
        public float Mass { get; set; } = 1f;
        public Vector2 Impulse { get; set; }
        public Vector2 Force { get; set; }
        public float Friction { get; set; }

        public Texture2D? Texture { get; set; }
        public Color Color { get; set; } = Color.White;
        public Vector2 Origin { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public float Rotation { get; set; }
        public SpriteEffects Effects { get; set; }
    }
}
