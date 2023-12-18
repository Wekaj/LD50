using LD50.Content;
using LD50.Entities;
using LD50.Graphics;
using LD50.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LD50.Levels {
    public class WorldDrawer(
        World world,
        IGraphicsDeviceSource graphicsDeviceSource,
        IContentManager content,
        InterfaceActions interfaceActions)
        : IDrawable {

        private readonly Lazy<SpriteBatch> _spriteBatch = new(() => new SpriteBatch(graphicsDeviceSource.GraphicsDevice));

        private readonly List<Entity> _drawingEntities = [];

        private Texture2D PixelTexture => content.Load<Texture2D>("Textures/pixel");
        private Texture2D CircleTexture => content.Load<Texture2D>("Textures/circle");
        private SpriteFont Font => content.Load<SpriteFont>("Fonts/font");

        public void Draw() {
            if (world.CurrentLevel is not null) {
                _spriteBatch.Value.Begin(transformMatrix: Matrix.CreateTranslation(new Vector3(-world.CurrentLevel.Position, 0f)));

                for (int i = 0; i < world.Levels.Count; i++) {
                    Level level = world.Levels[i];

                    for (int j = 0; j < level.Units.Count; j++) {
                        DrawUnitPath(level.Units[j]);
                    }
                }

                for (int i = 0; i < world.CurrentLevel.Units.Count; i++) {
                    DrawEntityShadow(world.CurrentLevel.Units[i].Entity);
                }
                for (int i = 0; i < world.CurrentLevel.Projectiles.Count; i++) {
                    DrawEntityShadow(world.CurrentLevel.Projectiles[i].Entity);
                }
                for (int i = 0; i < world.CurrentLevel.Fields.Count; i++) {
                    DrawField(world.CurrentLevel.Fields[i]);
                }

                _drawingEntities.AddRange(world.CurrentLevel.Units
                    .Select(unit => unit.Entity)
                    .Concat(world.CurrentLevel.Projectiles.Select(projectile => projectile.Entity))
                    .OrderBy(entity => entity.Position.Y));
                for (int i = 0; i < _drawingEntities.Count; i++) {
                    DrawEntity(_drawingEntities[i]);
                }
                _drawingEntities.Clear();

                for (int i = 0; i < world.CurrentLevel.Units.Count; i++) {
                    DrawUnitOverlay(world.CurrentLevel.Units[i]);
                }

                _spriteBatch.Value.End();
            }

            interfaceActions.DrawInterface(world);
        }

        private void DrawEntity(Entity entity) {
            if (entity.Texture is null) {
                return;
            }

            var texture = content.Load<Texture2D>(entity.Texture);

            _spriteBatch.Value.Draw(
                texture,
                entity.Position + new Vector2(0f, -entity.Depth),
                null,
                entity.Color,
                entity.Rotation,
                texture.Bounds.Size.ToVector2() * entity.Origin,
                entity.Scale,
                entity.Effects,
                0f);
        }

        private void DrawEntityShadow(Entity entity) {
            _spriteBatch.Value.Draw(
                CircleTexture,
                entity.Position,
                null,
                Color.Black * 0.25f,
                0f,
                new Vector2(CircleTexture.Width / 2f, CircleTexture.Height / 2f),
                new Vector2(0.5f, 0.25f),
                SpriteEffects.None,
                0f);
        }

        private void DrawField(Field field) {
            _spriteBatch.Value.Draw(
                CircleTexture,
                field.Entity.Position,
                null,
                Color.Red * 0.5f,
                0f,
                new Vector2(CircleTexture.Width / 2f, CircleTexture.Height / 2f),
                new Vector2(field.Radius * 2f / CircleTexture.Width, field.Radius * 2f / CircleTexture.Height),
                SpriteEffects.None,
                0f);
        }

        private void DrawUnitPath(Unit unit) {
            if (!unit.DrawPath || !unit.TargetPosition.HasValue) {
                return;
            }

            _spriteBatch.Value.Draw(
                PixelTexture,
                unit.Entity.Position,
                null,
                Color.Black,
                GetAngle(unit.TargetPosition.Value - unit.Entity.Position),
                new Vector2(0f, 0.5f),
                new Vector2(Vector2.Distance(unit.Entity.Position, unit.TargetPosition.Value), 1f),
                SpriteEffects.None,
                0f);
        }

        private void DrawUnitOverlay(Unit unit) {
            if (unit.Health >= unit.MaxHealth) {
                return;
            }

            const float healthBarWidth = 40f;
            const float healthBarHeight = 2f;

            Vector2 healthBarPosition = unit.Entity.Position + new Vector2(-healthBarWidth / 2f, -70f);

            _spriteBatch.Value.Draw(
                PixelTexture,
                healthBarPosition,
                null,
                Color.Black,
                0f,
                Vector2.Zero,
                new Vector2(healthBarWidth, healthBarHeight),
                SpriteEffects.None,
                0f);

            _spriteBatch.Value.Draw(
                PixelTexture,
                healthBarPosition,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                new Vector2(healthBarWidth * unit.PreviousHealth / unit.MaxHealth, healthBarHeight),
                SpriteEffects.None,
                0f);

            _spriteBatch.Value.Draw(
                PixelTexture,
                healthBarPosition,
                null,
                Color.Red,
                0f,
                Vector2.Zero,
                new Vector2(healthBarWidth * unit.Health / unit.MaxHealth, healthBarHeight),
                SpriteEffects.None,
                0f);
        }

        private static float GetAngle(Vector2 vector) {
            return (float)Math.Atan2(vector.Y, vector.X);
        }
    }
}
