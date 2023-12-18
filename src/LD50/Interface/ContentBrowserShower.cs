using LD50.Development;
using LD50.Levels;
using LD50.Screens;
using Microsoft.Xna.Framework;
using System;
using System.IO;

namespace LD50.Interface {
    public class ContentBrowserShower(
        World world,
        EngineEnvironment engineEnvironment) {

        public void ShowContentBrowser(string type, string directory, string extension, Action<string> callback) {
            if (engineEnvironment.ProjectDirectory is null) {
                return;
            }

            var popup = new Popup();

            popup.Elements.Add(new Element {
                Position = new Vector2(0f, 0f),
                Size = new Vector2(GameProperties.ScreenWidth, GameProperties.ScreenHeight),
                BackgroundColor = Color.DimGray * 0.85f,
            });

            popup.Elements.Add(new Element {
                Position = new Vector2(GameProperties.ScreenWidth - 200f - 8f, 8f),
                Size = new Vector2(200f, 20f),
                Label = "Cancel",
                OnClick = () => world.Popups.Remove(popup),
            });

            popup.Elements.Add(new Element {
                Position = new Vector2(GameProperties.ScreenWidth * 0.5f - 100f, 8f),
                Size = new Vector2(200f, 20f),
                Label = $"{type} Browser",
            });

            string fullDirectory = Path.Combine(engineEnvironment.ProjectDirectory, @"res\", directory);
            string[] contentFiles = Directory.GetFiles(fullDirectory, $"*{extension}");

            var position = new Vector2(8f, 8f + 28f);

            for (int i = 0; i < contentFiles.Length; i++) {
                string contentFile = contentFiles[i];
                string contentName = Path.ChangeExtension(Path.GetRelativePath(fullDirectory, contentFile), null);

                popup.Elements.Add(new Element {
                    Position = position,
                    Size = new Vector2(300f, 20f),
                    Label = contentName,
                    OnClick = () => {
                        world.Popups.Remove(popup);

                        callback(Path.GetRelativePath(Path.Combine(engineEnvironment.ProjectDirectory, @"res\"), contentFile));
                    },
                });

                position.Y += 22f;
                if (position.Y + 25f > GameProperties.ScreenHeight) {
                    position.Y = 8f + 28f;
                    position.X += 308f;
                }
            }

            world.Popups.Add(popup);
        }

        public void ShowTextureBrowser(Action<string> callback) {
            ShowContentBrowser(
                "Texture",
                @"Textures\",
                ".png",
                texture => {
                    texture = Path.ChangeExtension(texture, null);

                    callback(texture);
                });
        }

        public void ShowUnitBrowser(Action<string> callback) {
            ShowContentBrowser(
                "Unit",
                @"Units\",
                ".json",
                callback);
        }
    }
}
