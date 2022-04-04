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

        private readonly List<Level> _levels = new();
        private Level _currentLevel;

        public GameScreen(ContentManager content, SpriteBatch spriteBatch, InputBindings bindings) {
            _spriteBatch = spriteBatch;
            _bindings = bindings;

            _unitTexture = content.Load<Texture2D>("Textures/Character Test 1");
            _font = content.Load<SpriteFont>("Fonts/font");
            
            var random = new Random();
            for (int i = 0; i < 4; i++) {
                var level = new Level();

                for (int j = 0; j < 10; j++) {
                    level.Entities.Add(new Entity {
                        Position = new Vector2(random.Next(0, 800), random.Next(0, 600)),
                        Texture = _unitTexture,
                        Origin = new Vector2(_unitTexture.Width / 2, _unitTexture.Height),
                        Scale = new Vector2(0.4f),
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
            for (int i = 0; i < 4; i++) {
                if (_bindings.JustPressed(BindingId.Level1 + i)) {
                    _currentLevel = _levels[i];
                }
            }
        }

        public void Draw(GameTime gameTime) {
            _spriteBatch.Begin();

            for (int i = 0; i < _currentLevel.Entities.Count; i++) {
                DrawEntity(_currentLevel.Entities[i]);
            }

            for (int i = 0; i < _levels.Count; i++) {
                Level level = _levels[i];

                _spriteBatch.DrawString(_font, level.Name, new Vector2(8f + 160f * i, 8f), Color.Black);
            }

            _spriteBatch.End();
        }

        private void DrawEntity(Entity entity) {
            if (entity.Texture is null) {
                return;
            }

            _spriteBatch.Draw(
                entity.Texture,
                entity.Position,
                null,
                Color.White,
                0f,
                entity.Origin,
                entity.Scale,
                SpriteEffects.None,
                0f);
        }
    }
}
