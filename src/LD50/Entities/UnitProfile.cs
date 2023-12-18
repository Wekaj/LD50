﻿using System.Collections.Generic;
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
        public int Cost { get; set; }
        public List<string> StrongEnemyQuotes { get; init; } = [];
        public string? Minion1 { get; init; }
        public string? Minion2 { get; init; }
        public float Mass { get; set; } = 1f;
        public string? Texture { get; init; }
        public int Health { get; set; }
        public float VisionRange { get; set; }
        public float AttackRange { get; set; }
        public int AttackDamage { get; set; }
        public float AttackStun { get; set; }
        public int AttackTicks { get; set; } = 1;
        public float AttackCooldown { get; set; }
        public bool ThrowsMolotovs { get; set; }
        public UnitAnimation AttackingAnimation { get; init; }
        public Formation Formation { get; set; }
    }
}
