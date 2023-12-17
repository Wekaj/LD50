using LD50.Graphics;
using LD50.Levels;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace LD50.Entities {
    public record Unit {
        public Entity Entity { get; } = new();

        public string Name { get; set; } = "";
        public string? Portrait { get; set; }
        
        public Level? CurrentLevel { get; set; }

        public string? Dialogue { get; set; }
        public float DialogueTimer { get; set; }
        public List<string> StrongEnemyQuotes { get; } = new();

        public float Direction { get; set; }
        public bool PrioritisesTargetPosition { get; set; }

        public ActiveAnimation? Animation { get; set; }
        public string? DefaultTexture { get; set; }

        public Vector2? TargetPosition { get; set; }
        public Unit? TargetUnit { get; set; }
        public float WalkTimer { get; set; }
        public bool DrawPath { get; set; }

        public Team Team { get; set; }
        public int MaxHealth { get; set; }
        public int Health { get; set; }

        public float PreviousHealth { get; set; }
        public float PreviousHealthTimer { get; set; }

        public float VisionRange { get; set; }
        public float AttackRange { get; set; }
        public int AttackDamage { get; set; }
        public float AttackStun { get; set; }
        public int AttackTicks { get; set; } = 1;
        public float AttackCooldown { get; set; }
        public bool ThrowsMolotovs { get; set; }

        public float AttackTickTimer { get; set; }
        public int RemainingTicks { get; set; }
        public Unit? AttackingUnit { get; set; }

        public float CooldownTimer { get; set; }
        public Animation? AttackingAnimation { get; set; }

        public Unit? Commander { get; set; }
        public List<Unit> Minions { get; } = new();
        public Formation Formation { get; set; }
    }
}
