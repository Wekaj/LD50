using LD50.Graphics;
using LD50.Levels;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Text.Json;

namespace LD50.Entities {
    public class UnitFactory(AnimationManager animations) {
        private readonly Random _random = new();

        public Unit CreateUnit(string unitProfileReference) {
            using FileStream stream = File.OpenRead(unitProfileReference);

            var unitProfile = JsonSerializer.Deserialize<UnitProfile>(stream)!;

            return new Unit {
                Name = unitProfile.Name ?? "",
                Portrait = unitProfile.Portrait,
                StrongEnemyQuotes = unitProfile.StrongEnemyQuotes,
                Minion1 = unitProfile.Minion1,
                Minion2 = unitProfile.Minion2,

                Entity = {
                    Friction = 500f,
                    Mass = unitProfile.Mass,

                    Texture = unitProfile.Texture,
                    Origin = new Vector2(0.5f, 1f),
                    Scale = new Vector2(0.75f),
                },

                DefaultTexture = unitProfile.Texture,

                MaxHealth = unitProfile.Health,
                Health = unitProfile.Health,

                VisionRange = unitProfile.VisionRange,
                AttackRange = unitProfile.AttackRange,
                AttackDamage = unitProfile.AttackDamage,
                AttackStun = unitProfile.AttackStun,
                AttackTicks = unitProfile.AttackTicks,
                AttackCooldown = unitProfile.AttackCooldown,
                ThrowsMolotovs = unitProfile.ThrowsMolotovs,

                AttackingAnimation = animations.Animations[unitProfile.AttackingAnimation],

                Formation = unitProfile.Formation,
            };
        }

        public Unit CreateCommander(string unitProfileReference) {
            Unit unit = CreateUnit(unitProfileReference);

            unit.Entity.Position = new Vector2(_random.Next(0, 800), _random.Next(0, 600));
            
            unit.PrioritisesTargetPosition = true;
            unit.DrawPath = true;

            return unit;
        }

        public Unit CreateUnit(Level level) {
            Unit unit = _random.Next(2) == 0
                ? CreateUnit(@"Units\gunner.json")
                : CreateUnit(@"Units\batter.json");

            unit.Entity.Position = level.Position + new Vector2(_random.Next(0, 800), _random.Next(0, 600));

            return unit;
        }
    }
}
