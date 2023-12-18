using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LD50.Entities {
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UnitAnimation {
        GunnerAttacking,
        BatterAttacking,
        RifleWomanAttacking,
        PistolWomanAttacking,
        MinigunLieutenantAttacking,
        DaggerLieutenantAttacking,
        MolotovLieutenantAttacking
    }

    public record UnitProfile {
        public string? Name { get; init; }
        public string? Portrait { get; init; }
        public List<string> StrongEnemyQuotes { get; init; } = [];
        public float Mass { get; init; } = 1f;
        public string? Texture { get; init; }
        public int Health { get; set; }
        public float VisionRange { get; init; }
        public float AttackRange { get; init; }
        public int AttackDamage { get; set; }
        public float AttackStun { get; init; }
        public int AttackTicks { get; set; } = 1;
        public float AttackCooldown { get; init; }
        public bool ThrowsMolotovs { get; set; }
        public UnitAnimation AttackingAnimation { get; init; }
        public Formation Formation { get; init; }
    }
}
