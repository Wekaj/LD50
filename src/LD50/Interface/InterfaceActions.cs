using LD50.Entities;
using LD50.Input;
using LD50.Levels;
using LD50.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LD50.Interface {
    public class InterfaceActions {
        private readonly XnaMouse _mouse;
        private readonly InputBindings _bindings;
        private readonly SpriteBatch _spriteBatch;

        private readonly Texture2D _pixelTexture;
        private readonly SpriteFont _font;

        public InterfaceActions(
            XnaMouse mouse,
            InputBindings bindings,
            SpriteBatch spriteBatch,
            ContentManager content) {

            _mouse = mouse;
            _bindings = bindings;
            _spriteBatch = spriteBatch;

            _pixelTexture = content.Load<Texture2D>("Textures/pixel");
            _font = content.Load<SpriteFont>("Fonts/font");
        }

        public bool HandleMouseClick(World world) {
            for (int i = 0; i < world.Elements.Count; i++) {
                Element element = world.Elements[i];

                if (element.OnClick is not null && MouseIntersectsElement(element)) {
                    element.OnClick.Invoke();
                    return true;
                }
            }

            return false;
        }

        public void DrawInterface(World world) {
            _spriteBatch.Begin();

            string moneyString = $"${world.PlayerMoney}";
            _spriteBatch.DrawString(_font, moneyString, new Vector2(8f, 600f - 8f - 50f - 8f - _font.MeasureString(moneyString).Y), Color.Black);

            for (int i = 0; i < world.Elements.Count; i++) {
                DrawElement(world.Elements[i]);
            }
            
            for (int i = 0; i < world.Commanders.Count; i++) {
                Entity commander = world.Commanders[i];

                if (commander.Dialogue is null) {
                    continue;
                }

                Vector2 dialogueSize = _font.MeasureString(commander.Dialogue);

                _spriteBatch.Draw(
                    _pixelTexture,
                    new Vector2(800f - 8f - 100f - dialogueSize.X, 8f + (60f + 8f) * i),
                    null,
                    Color.Black * 0.5f,
                    0f,
                    Vector2.Zero,
                    dialogueSize,
                    SpriteEffects.None,
                    0f);

                _spriteBatch.DrawString(
                    _font,
                    commander.Dialogue,
                    new Vector2(800f - 8f - 100f - dialogueSize.X, 8f + (60f + 8f) * i),
                    Color.White);
            }

            _spriteBatch.End();
        }

        private void DrawElement(Element element) {
            Color backgroundColor = Color.Black * 0.5f;
            Color labelColor = Color.White;

            if (element.IsHighlighted()) {
                backgroundColor = Color.White * 0.5f;
                labelColor = Color.Black;
            }

            if (element.OnClick is not null && MouseIntersectsElement(element)) {
                backgroundColor = Color.White * 0.5f;
                labelColor = Color.Black;

                if (_bindings.IsPressed(BindingId.Select)) {
                    backgroundColor *= 0.5f;
                    labelColor *= 0.5f;
                }
            }

            _spriteBatch.Draw(
                _pixelTexture,
                element.Position,
                null,
                backgroundColor,
                0f,
                Vector2.Zero,
                element.Size,
                SpriteEffects.None,
                0f);

            if (element.IsTextBlock) {
                _spriteBatch.DrawString(
                    _font,
                    element.Label.WrapText(_font, element.Size.X - element.Margin * 2f),
                    Vector2.Floor(element.Position + new Vector2(element.Margin)),
                    labelColor);
            }
            else {
                Vector2 labelSize = _font.MeasureString(element.Label);

                float height = labelSize.Y;

                if (element.Image is not null) {
                    height += element.Image.Height;
                }

                Vector2 position = element.Position + element.Size / 2f - new Vector2(0f, height / 2f);

                if (element.Image is not null) {
                    _spriteBatch.Draw(
                        element.Image,
                        Vector2.Floor(position - new Vector2(element.Image.Width / 2f, 0f)),
                        null,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0f);

                    position.Y += element.Image.Height;
                }
                
                _spriteBatch.DrawString(
                    _font,
                    element.Label,
                    Vector2.Floor(position - new Vector2(labelSize.X / 2f, 0f)),
                    labelColor);
            }
        }

        private bool MouseIntersectsElement(Element element) {
            return _mouse.Position.X >= element.Position.X
                && _mouse.Position.X <= element.Position.X + element.Size.X
                && _mouse.Position.Y >= element.Position.Y
                && _mouse.Position.Y <= element.Position.Y + element.Size.Y;
        }
    }
}
