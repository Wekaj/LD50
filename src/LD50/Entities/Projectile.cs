using Microsoft.Xna.Framework;

namespace LD50.Entities {
    public record Projectile {
        public Entity Entity { get; } = new();

        public Unit? Source { get; set; }
        
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }
        public float Peak { get; set; }
        public float TravelDuration { get; set; }
        public float TravelTimer { get; set; }

        public float RotationSpeed { get; set; }
        public Field? Field { get; set; }
    }
}
