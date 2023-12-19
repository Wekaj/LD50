using LD50.Content;
using LD50.Entities;
using LD50.Graphics;
using LD50.Input;
using LD50.Levels;
using LD50.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace LD50.Interface {
    public class InterfaceActions(
        World world,
        XnaMouse mouse,
        InputBindings bindings,
        IGraphicsDeviceSource graphicsDeviceSource,
        IDeltaTimeSource deltaTimeSource,
        IContentManager content) {

        private readonly Lazy<SpriteBatch> _spriteBatch = new(() => new SpriteBatch(graphicsDeviceSource.GraphicsDevice));

        private Texture2D PixelTexture => content.Load<Texture2D>("Textures/pixel");
        private SpriteFont Font => content.Load<SpriteFont>("Fonts/font");

        public void Update() {
            UpdateElements(world.Elements, world.Popups.Count == 0);

            for (int i = 0; i < world.Popups.Count; i++) {
                UpdateElements(world.Popups[i].Elements, i == world.Popups.Count - 1);
            }
        }

        public bool HandleMouseClick() {
            List<Element> activeElements = GetActiveElements();

            for (int i = 0; i < activeElements.Count; i++) {
                Element element = activeElements[i];

                if (element.IsVisible() && element.OnClick is not null && MouseIntersectsElement(element)) {
                    element.OnClick.Invoke();
                    return true;
                }
            }

            return false;
        }

        public void DrawInterface() {
            _spriteBatch.Value.Begin();

            string moneyString = $"${world.PlayerMoney}";
            _spriteBatch.Value.DrawString(Font, moneyString, new Vector2(8f, 600f - 8f - 50f - 8f - Font.MeasureString(moneyString).Y), Color.Black);

            DrawElements(world.Elements, world.Popups.Count == 0);
            
            for (int i = 0; i < world.Commanders.Count; i++) {
                Unit commander = world.Commanders[i];

                if (commander.Dialogue is null) {
                    continue;
                }

                Vector2 dialogueSize = Font.MeasureString(commander.Dialogue);

                _spriteBatch.Value.Draw(
                    PixelTexture,
                    new Vector2(960f - 8f - 120f - dialogueSize.X, 8f + (120f + 8f) * i),
                    null,
                    Color.Black * 0.5f,
                    0f,
                    Vector2.Zero,
                    dialogueSize,
                    SpriteEffects.None,
                    0f);

                _spriteBatch.Value.DrawString(
                    Font,
                    commander.Dialogue,
                    new Vector2(960f - 8f - 120f - dialogueSize.X, 8f + (120f + 8f) * i),
                    Color.White);
            }

            for (int i = 0; i < world.Popups.Count; i++) {
                DrawElements(world.Popups[i].Elements, i == world.Popups.Count - 1);
            }

            _spriteBatch.Value.End();
        }

        private List<Element> GetActiveElements() {
            if (world.Popups.Count == 0) {
                return world.Elements;
            }

            return world.Popups[^1].Elements;
        }

        private void UpdateElements(List<Element> elements, bool groupIsActive) {
            for (int i = 0; i < elements.Count; i++) {
                Element element = elements[i];

                if (groupIsActive && element.IsVisible() && element.OnClick is not null && element.Binding.HasValue && bindings.JustReleased(element.Binding.Value)) {
                    element.OnClick.Invoke();
                }

                if (element.Animation is not null) {
                    element.Animation.Update(deltaTimeSource.Latest);
                    element.Image = element.Animation.Apply() ?? element.Image;
                }
            }
        }

        private void DrawElements(List<Element> elements, bool groupIsActive) {
            for (int i = 0; i < elements.Count; i++) {
                DrawElement(elements[i], groupIsActive);
            }
        }

        private void DrawElement(Element element, bool groupIsActive) {
            if (!element.IsVisible()) {
                return;
            }
            
            Color backgroundColor = element.BackgroundColor;
            Color labelColor = Color.White;

            if (element.IsHighlighted()) {
                backgroundColor = Color.White * 0.5f;
                labelColor = Color.Black;
            }

            if (groupIsActive && element.OnClick is not null) {
                bool mouseIntersects = MouseIntersectsElement(element);

                if (mouseIntersects) {
                    backgroundColor = Color.White * 0.5f;
                    labelColor = Color.Black;
                }

                if ((mouseIntersects && bindings.IsPressed(BindingId.Select)) || (element.Binding.HasValue && bindings.IsPressed(element.Binding.Value))) {
                    backgroundColor *= 0.5f;
                    labelColor *= 0.5f;
                }
            }

            _spriteBatch.Value.Draw(
                PixelTexture,
                element.Position,
                null,
                backgroundColor,
                0f,
                Vector2.Zero,
                element.Size,
                SpriteEffects.None,
                0f);

            if (element.IsTextBlock) {
                _spriteBatch.Value.DrawString(
                    Font,
                    element.Label.WrapText(Font, element.Size.X - element.Margin * 2f),
                    Vector2.Floor(element.Position + new Vector2(element.Margin)),
                    labelColor);
            }
            else {
                Vector2 labelSize = Font.MeasureString(element.Label);

                float height = labelSize.Y;

                Texture2D? image = element.Image is not null ? content.Load<Texture2D>(element.Image) : null;
                Vector2 imageScale = Vector2.One;
                if (image is not null) {
                    if (element.ResizeImageToContain && (image.Width > element.Size.X || image.Height + height > element.Size.Y)) {
                        if (image.Width > image.Height + height) {
                            imageScale = new Vector2(element.Size.X / image.Width);
                        }
                        else {
                            imageScale = new Vector2(element.Size.Y / (image.Height + height));
                        }
                    }

                    height += image.Height * imageScale.Y;
                }

                Vector2 position = element.Position + element.Size / 2f - new Vector2(0f, height / 2f);

                if (image is not null) {
                    _spriteBatch.Value.Draw(
                        image,
                        Vector2.Floor(position - new Vector2(image.Width * imageScale.X / 2f, 0f)),
                        null,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        imageScale,
                        SpriteEffects.None,
                        0f);
                    
                    position.Y += image.Height * imageScale.Y;
                }
                
                _spriteBatch.Value.DrawString(
                    Font,
                    element.Label,
                    Vector2.Floor(position - new Vector2(labelSize.X / 2f, 0f)),
                    labelColor);
            }
        }

        private bool MouseIntersectsElement(Element element) {
            return mouse.Position.X >= element.Position.X
                && mouse.Position.X <= element.Position.X + element.Size.X
                && mouse.Position.Y >= element.Position.Y
                && mouse.Position.Y <= element.Position.Y + element.Size.Y;
        }
    }
}
