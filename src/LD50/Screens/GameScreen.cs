using LD50.Entities;
using LD50.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LD50.Screens {
    public class GameScreen : IScreen {
        private readonly SpriteBatch _spriteBatch;

        private Level _currentLevel = new();

        public GameScreen(ContentManager content, SpriteBatch spriteBatch) {
            _spriteBatch = spriteBatch;
            
            var random = new Random();
            for (int i = 0; i < 10; i++) {
                _currentLevel.Entities.Add(new Entity {
                    Position = new Vector2(random.Next(0, 800), random.Next(0, 600)),
                    Texture = content.Load<Texture2D>("Textures/unit"),
                });
            }
        }

        public void Update(GameTime gameTime) {
        }

        public void Draw(GameTime gameTime) {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            for (int i = 0; i < _currentLevel.Entities.Count; i++) {
                DrawEntity(_currentLevel.Entities[i]);
            }

            _spriteBatch.End();
        }

        private void DrawEntity(Entity entity) {
            _spriteBatch.Draw(entity.Texture, entity.Position, Color.White);
        }
    }
}
