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

        private readonly Texture2D _unitTexture;
        private readonly SpriteFont _font;

        private readonly Random _random = new();

        private readonly List<Level> _levels = new();
        private Level _currentLevel;

        public GameScreen(ContentManager content, SpriteBatch spriteBatch, InputBindings bindings) {
            _spriteBatch = spriteBatch;
            _bindings = bindings;

            _unitTexture = content.Load<Texture2D>("Textures/Character Test 3");
            _font = content.Load<SpriteFont>("Fonts/font");
            
            for (int i = 0; i < 4; i++) {
                var level = new Level();

                int units = _random.Next(2, 10);
                for (int j = 0; j < units; j++) {
                    level.Entities.Add(new Entity {
                        Position = new Vector2(_random.Next(0, 800), _random.Next(0, 600)),
                        Texture = _unitTexture,
                        Origin = new Vector2(_unitTexture.Width / 2, _unitTexture.Height),
                        Scale = new Vector2(0.75f),
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

        private void UpdateLevel(Level level, float deltaTime) {
            for (int i = 0; i < level.Entities.Count; i++) {
                Entity entity = level.Entities[i];

                if (_random.Next(1000) == 0) {
                    entity.TargetPosition = new Vector2(_random.Next(0, 800), _random.Next(0, 600));
                }

                if (entity.TargetPosition.HasValue) {
                    float distance = Vector2.Distance(entity.Position, entity.TargetPosition.Value);
                    float speed = 100f * deltaTime;
                    if (distance < speed) {
                        entity.Position = entity.TargetPosition.Value;
                        entity.TargetPosition = null;
                    }
                    else {
                        entity.Position += (entity.TargetPosition.Value - entity.Position) * (speed / distance);

                        if (entity.TargetPosition.Value.X > entity.Position.X) {
                            entity.Effects = SpriteEffects.None;
                        }
                        else {
                            entity.Effects = SpriteEffects.FlipHorizontally;
                        }

                        entity.HopTimer += 15f * deltaTime;
                    }
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
                Color.White,
                0f,
                entity.Origin,
                entity.Scale,
                entity.Effects,
                0f);
        }
    }
}
