using LD50.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD50.Entities {
    public record Entity {
        public string Name { get; set; } = "";

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Mass { get; set; } = 1f;
        public Vector2 Impulse { get; set; }
        public Vector2 Force { get; set; }
        public float Friction { get; set; }

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
        public bool IsWanderer { get; set; }
        public bool DrawPath { get; set; }

        public Team Team { get; set; }
        public int MaxHealth { get; set; }
        public int Health { get; set; }

        public float PreviousHealth { get; set; }
        public float PreviousHealthTimer { get; set; }

        public float AttackRange { get; set; }
        public int AttackDamage { get; set; }
        public float AttackStun { get; set; }
        public int AttackTicks { get; set; } = 1;
        public float AttackCooldown { get; set; }

        public float AttackTickTimer { get; set; }
        public int RemainingTicks { get; set; }
        public Entity? AttackingEntity { get; set; }

        public float CooldownTimer { get; set; }
        public Animation? AttackingAnimation { get; set; }

        public Entity? Commander { get; set; }
    }
}
