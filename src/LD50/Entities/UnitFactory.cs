using LD50.Graphics;
using LD50.Levels;
using Microsoft.Xna.Framework;
using System;

namespace LD50.Entities {
    public class UnitFactory(AnimationManager animations) {
        private readonly Random _random = new();

        public Unit CreateUnit(Level level) {
            Unit unit = _random.Next(2) == 0
                ? CreateGunner()
                : CreateBatter();

            unit.Entity.Position = level.Position + new Vector2(_random.Next(0, 800), _random.Next(0, 600));

            return unit;
        }

        public Unit CreateGunner() {
            return new Unit {
                Entity = {
                    Friction = 500f,

                    Texture = "Textures/Gunner Test 1",
                    Origin = new Vector2(0.5f, 1f),
                    Scale = new Vector2(0.75f),
                },

                DefaultTexture = "Textures/Gunner Test 1",

                MaxHealth = 80,
                Health = 80,

                VisionRange = 200f,
                AttackRange = 150f,
                AttackDamage = 10,
                AttackStun = 0.025f,
                AttackTicks = 3,
                AttackCooldown = 2f,

                AttackingAnimation = animations.GunnerAttacking,

                Formation = Formation.Group,
            };
        }

        public Unit CreateBatter() {
            return new Unit {
                Entity = {
                    Friction = 500f,

                    Texture = "Textures/Batter Test 1",
                    Origin = new Vector2(0.5f, 1f),
                    Scale = new Vector2(0.75f),
                },

                DefaultTexture = "Textures/Batter Test 1",

                MaxHealth = 100,
                Health = 100,

                VisionRange = 200f,
                AttackRange = 50f,
                AttackDamage = 10,
                AttackStun = 0.25f,
                AttackCooldown = 1f,

                AttackingAnimation = animations.BatterAttacking,

                Formation = Formation.FrontArc,
            };
        }

        public Unit CreateRifleWoman() {
            return new Unit {
                Entity = {
                    Friction = 500f,

                    Texture = "Textures/RifleWoman Test 1",
                    Origin = new Vector2(0.5f, 1f),
                    Scale = new Vector2(0.75f),
                },

                DefaultTexture = "Textures/RifleWoman Test 1",

                MaxHealth = 60,
                Health = 60,

                VisionRange = 250f,
                AttackRange = 250f,
                AttackDamage = 60,
                AttackStun = 0.5f,
                AttackCooldown = 3f,

                AttackingAnimation = animations.RifleWomanAttacking,

                Formation = Formation.Group,
            };
        }

        public Unit CreatePistolWoman() {
            return new Unit {
                Entity = {
                    Friction = 500f,

                    Texture = "Textures/PistolWoman Test 1",
                    Origin = new Vector2(0.5f, 1f),
                    Scale = new Vector2(0.75f),
                },

                DefaultTexture = "Textures/PistolWoman Test 1",

                MaxHealth = 70,
                Health = 70,

                VisionRange = 200f,
                AttackRange = 100f,
                AttackDamage = 3,
                AttackStun = 0.025f,
                AttackTicks = 10,
                AttackCooldown = 2f,

                AttackingAnimation = animations.PistolWomanAttacking,

                Formation = Formation.FrontArc,
            };
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

                AttackingAnimation = animations.MinigunLieutenantAttacking,

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

                AttackingAnimation = animations.DaggerLieutenantAttacking,

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

                AttackingAnimation = animations.MolotovLieutenantAttacking,

                DrawPath = true,
            };
        }
    }
}
