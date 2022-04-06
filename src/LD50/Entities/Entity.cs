using LD50.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD50.Entities {
    public record Entity {
        public Vector2 Position { get; set; }

        public Texture2D? Texture { get; set; }
        public Color Color { get; set; } = Color.White;
        public Vector2 Origin { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public SpriteEffects Effects { get; set; }

        public ActiveAnimation? Animation { get; set; }
        public Texture2D? DefaultTexture { get; set; }

        public Vector2? TargetPosition { get; set; }
        public Entity? TargetEntity { get; set; }
        public float WalkTimer { get; set; }

        public Team Team { get; set; }
        public int MaxHealth { get; set; }
        public int Health { get; set; }

        public float AttackRange { get; set; }
        public int AttackDamage { get; set; }
        public float AttackInterval { get; set; }

        public float AttackTimer { get; set; }
        public Animation? AttackingAnimation { get; set; }
    }
}
