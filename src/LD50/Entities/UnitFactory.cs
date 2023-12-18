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
                Entity = {
                    Friction = 500f,

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

                AttackingAnimation = animations.Animations[unitProfile.AttackingAnimation],

                Formation = unitProfile.Formation,
            };
        }

        public Unit CreateUnit(Level level) {
            Unit unit = _random.Next(2) == 0
                ? CreateUnit(@"Units\gunner.json")
                : CreateUnit(@"Units\batter.json");

            unit.Entity.Position = level.Position + new Vector2(_random.Next(0, 800), _random.Next(0, 600));

            return unit;
        }

        public Unit CreateMinigunLieutenant() {
            return new Unit {
                Name = "Alphonso",
                Portrait = "Textures/portrait_alphonso",

                StrongEnemyQuotes = {
                    "Everyone, prepare yourself!",
                    "We need all the firepower we can get for this one.",
                    "This is gonna hurt.",
                },

                Entity = {
                    Position = new Vector2(_random.Next(0, 800), _random.Next(0, 600)),
                    Friction = 500f,
                    Mass = 5f,

                    Texture = "Textures/Lieutenant Test 1",
                    Origin = new Vector2(0.5f, 1f),
                    Scale = new Vector2(0.75f),
                },

                PrioritisesTargetPosition = true,

                DefaultTexture = "Textures/Lieutenant Test 1",

                MaxHealth = 300,
                Health = 300,

                VisionRange = 300f,
                AttackRange = 300f,
                AttackDamage = 15,
                AttackStun = 0.025f,
                AttackTicks = 5,
                AttackCooldown = 3f,

                AttackingAnimation = animations.Animations[UnitAnimation.MinigunLieutenantAttacking],

                DrawPath = true,
            };
        }

        public Unit CreateDaggerLieutenant() {
            return new Unit {
                Name = "Marissa",
                Portrait = "Textures/Lieutenant2_Portrait2 Test 1",

                StrongEnemyQuotes = {
                    "No time for breaks, huh...",
                    "This won't be easy.",
                    "I'll need an extra sharp blade for this one...",
                },

                Entity = {
                    Position = new Vector2(_random.Next(0, 800), _random.Next(0, 600)),
                    Friction = 500f,
                    Mass = 5f,

                    Texture = "Textures/Lieutenant2 Test 1",
                    Origin = new Vector2(0.5f, 1f),
                    Scale = new Vector2(0.75f),
                },

                PrioritisesTargetPosition = true,

                DefaultTexture = "Textures/Lieutenant2 Test 1",

                MaxHealth = 200,
                Health = 200,

                VisionRange = 300f,
                AttackRange = 300f,
                AttackDamage = 20,
                AttackStun = 0.025f,
                AttackCooldown = 0.5f,

                AttackingAnimation = animations.Animations[UnitAnimation.DaggerLieutenantAttacking],

                DrawPath = true,
            };
        }

        public Unit CreateMolotovLieutenant() {
            return new Unit {
                Name = "Pirro",
                Portrait = "Textures/portrait_pirro",

                StrongEnemyQuotes = {
                    "Uh oh, this can't be good!",
                    "Imma need more bombs for this one!",
                    "It's party time!",
                },

                Entity = {
                    Position = new Vector2(_random.Next(0, 800), _random.Next(0, 600)),
                    Friction = 500f,
                    Mass = 5f,

                    Texture = "Textures/Lieutenant3 Test 1",
                    Origin = new Vector2(0.5f, 1f),
                    Scale = new Vector2(0.75f),
                },

                PrioritisesTargetPosition = true,

                DefaultTexture = "Textures/Lieutenant3 Test 1",

                MaxHealth = 250,
                Health = 250,

                VisionRange = 300f,
                AttackRange = 300f,
                AttackDamage = 0,
                AttackStun = 0f,
                AttackTicks = 2,
                AttackCooldown = 4f,
                ThrowsMolotovs = true,

                AttackingAnimation = animations.Animations[UnitAnimation.MolotovLieutenantAttacking],

                DrawPath = true,
            };
        }
    }
}
