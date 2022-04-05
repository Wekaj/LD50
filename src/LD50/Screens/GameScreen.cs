using LD50.Entities;
using LD50.Input;
using LD50.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace LD50.Screens {
    public class GameScreen : IScreen {
        private readonly SpriteBatch _spriteBatch;
        private readonly InputBindings _bindings;

        private readonly Texture2D _unitTexture1;
        private readonly Texture2D _unitTexture2;
        private readonly SpriteFont _font;

        private readonly Random _random = new();

        private readonly List<Level> _levels = new();
        private Level _currentLevel;

        public GameScreen(ContentManager content, SpriteBatch spriteBatch, InputBindings bindings) {
            _spriteBatch = spriteBatch;
            _bindings = bindings;

            _unitTexture1 = content.Load<Texture2D>("Textures/Character Test 3");
            _unitTexture2 = content.Load<Texture2D>("Textures/Batter Test 1");
            _font = content.Load<SpriteFont>("Fonts/font");
            
            for (int i = 0; i < 4; i++) {
                var level = new Level();

                int units = _random.Next(2, 10);
                for (int j = 0; j < units; j++) {
                    level.Entities.Add(CreateUnit() with {
                        Team = Team.Player,
                    });
                }

                int enemies = _random.Next(2, 4);
                for (int j = 0; j < enemies; j++) {
                    level.Entities.Add(CreateUnit() with {
                        Team = Team.Enemy,
                        Color = Color.Red,
                    });
                }

                _levels.Add(level);
            }

            _levels[0].Name = "Family Restaurant";
            _levels[1].Name = "Back Alleys";
            _levels[2].Name = "Workrooms";
            _levels[3].Name = "Headquarters";

            _currentLevel = _levels[0];
        }

        public void Update(GameTime gameTime) {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < 4; i++) {
                if (_bindings.JustPressed(BindingId.Level1 + i)) {
                    _currentLevel = _levels[i];
                }
            }

            for (int i = 0; i < _levels.Count; i++) {
                UpdateLevel(_levels[i], deltaTime);
            }
        }

        public void Draw(GameTime gameTime) {
            _spriteBatch.Begin();

            for (int i = 0; i < _currentLevel.Entities.Count; i++) {
                DrawEntity(_currentLevel.Entities[i]);
            }

            for (int i = 0; i < _levels.Count; i++) {
                Level level = _levels[i];

                _spriteBatch.DrawString(_font, level.Name, new Vector2(8f + 160f * i, 8f), level == _currentLevel ? Color.White : Color.Black);
            }

            _spriteBatch.End();
        }

        private Entity CreateUnit() {
            return _random.Next(2) == 0
                ? CreateGunner()
                : CreateBatter();
        }

        private Entity CreateGunner() {
            return new Entity {
                Position = new Vector2(_random.Next(0, 800), _random.Next(0, 600)),

                Texture = _unitTexture1,
                Origin = new Vector2(_unitTexture1.Width / 2, _unitTexture1.Height),
                Scale = new Vector2(0.75f),

                Health = 80,

                AttackRange = 150f,
                AttackDamage = 30,
                AttackInterval = 2f,
            };
        }

        private Entity CreateBatter() {
            return new Entity {
                Position = new Vector2(_random.Next(0, 800), _random.Next(0, 600)),

                Texture = _unitTexture2,
                Origin = new Vector2(_unitTexture1.Width / 2, _unitTexture1.Height),
                Scale = new Vector2(0.75f),

                Health = 100,

                AttackRange = 50f,
                AttackDamage = 10,
                AttackInterval = 1f,
            };
        }

        private void UpdateLevel(Level level, float deltaTime) {
            for (int i = 0; i < level.Entities.Count; i++) {
                Entity entity = level.Entities[i];

                if (entity.Health <= 0) {
                    level.Entities.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < level.Entities.Count; i++) {
                UpdateEntity(level.Entities[i], level, deltaTime);
            }
        }

        private void UpdateEntity(Entity entity, Level level, float deltaTime) {
            if (entity.TargetEntity is not null && entity.TargetEntity.Health <= 0) {
                entity.TargetEntity = null;
            }

            if (entity.TargetEntity is null) {
                for (int j = 0; j < level.Entities.Count; j++) {
                    Entity other = level.Entities[j];

                    if (other.Team == entity.Team || Vector2.DistanceSquared(entity.Position, other.Position) > 200f * 200f) {
                        continue;
                    }

                    entity.TargetEntity = other;
                }
            }
            else if (Vector2.DistanceSquared(entity.Position, entity.TargetEntity.Position) <= entity.AttackRange * entity.AttackRange) {
                entity.AttackTimer += deltaTime;

                if (entity.AttackTimer >= entity.AttackInterval) {
                    entity.AttackTimer -= entity.AttackInterval;

                    entity.TargetEntity.Health -= entity.AttackDamage;
                    entity.TargetEntity.AttackTimer -= 0.25f;
                }
            }

            if (_random.Next(1000) == 0) {
                entity.TargetPosition = new Vector2(_random.Next(0, 800), _random.Next(0, 600));
            }

            Vector2? targetPosition = entity.TargetEntity?.Position ?? entity.TargetPosition;
            float? targetDistance = entity.TargetEntity is not null ? entity.AttackRange : null;

            if (targetPosition.HasValue) {
                float distance = Vector2.Distance(entity.Position, targetPosition.Value);
                float speed = 100f * deltaTime;
                if (distance < speed) {
                    entity.Position = targetPosition.Value;
                }
                else if (targetDistance is null || distance > targetDistance.Value) {
                    entity.Position += (targetPosition.Value - entity.Position) * (speed / distance);

                    if (targetPosition.Value.X > entity.Position.X) {
                        entity.Effects = SpriteEffects.None;
                    }
                    else {
                        entity.Effects = SpriteEffects.FlipHorizontally;
                    }

                    entity.HopTimer += 15f * deltaTime;
                }
            }
        }

        private void DrawEntity(Entity entity) {
            if (entity.Texture is null) {
                return;
            }

            _spriteBatch.Draw(
                entity.Texture,
                entity.Position + new Vector2(0f, -(float)Math.Abs(Math.Sin(entity.HopTimer)) * 10f),
                null,
                entity.Color,
                0f,
                entity.Origin,
                entity.Scale,
                entity.Effects,
                0f);

            string health = entity.Health.ToString();
            Vector2 healthSize = _font.MeasureString(health);
            _spriteBatch.DrawString(
                _font,
                health,
                entity.Position + new Vector2(-healthSize.X / 2f, -96f),
                Color.White);
        }
    }
}
