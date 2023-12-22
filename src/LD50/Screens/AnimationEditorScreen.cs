using LD50.Content;
using LD50.Development;
using LD50.Graphics;
using LD50.Interface;
using LD50.Levels;
using LD50.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace LD50.Screens {
    public class AnimationEditorScreen(
        World world,
        EngineEnvironment engineEnvironment,
        ScreenChanger screenChanger,
        ContentBrowserShower contentBrowserShower)
        : IScreen {

        private readonly List<Element> _selectionElements = [];

        private readonly JsonSerializerOptions _jsonSerializerOptions = new() {
            WriteIndented = true,
        };

        private string? _selectedAnimation;

        public void Show(ScreenArgs args) {
            world.Elements.Add(new Element {
                Position = new Vector2(GameProperties.ScreenWidth - 200f - 8f, 8f),
                Size = new Vector2(200f, 20f),
                Label = "Home",
                OnClick = () => screenChanger.ChangeScreen(new ScreenArgs(ScreenType.Engine)),
            });

            world.Elements.Add(new Element {
                Position = new Vector2(GameProperties.ScreenWidth * 0.5f - 100f, 8f),
                Size = new Vector2(200f, 20f),
                Label = "Animation Editor",
            });

            string[] animationFiles = Directory.GetFiles(engineEnvironment.AnimationDirectory, "*.json");

            for (int i = 0; i < animationFiles.Length; i++) {
                string animationFile = animationFiles[i];
                string animationName = Path.ChangeExtension(Path.GetRelativePath(engineEnvironment.AnimationDirectory, animationFile), null);

                world.Elements.Add(new Element {
                    Position = new Vector2(8f, 8f + 28f + 22f * i),
                    Size = new Vector2(300f, 20f),
                    Label = animationName,
                    OnClick = () => SelectAnimation(animationFile),
                    IsHighlighted = () => _selectedAnimation == animationFile,
                });
            }
        }

        public void Hide() {
            world.Reset();

            _selectedAnimation = null;
        }

        private void SelectAnimation(string animationFile) {
            _selectedAnimation = animationFile;

            Animation animation;
            using (FileStream stream = File.OpenRead(animationFile)) {
                animation = JsonSerializer.Deserialize<Animation>(stream, _jsonSerializerOptions)!;
            }

            SelectAnimation(animation, animationFile);
        }

        private void SelectAnimation(Animation animation, string animationFile) {
            world.Elements.RemoveWhere(_selectionElements.Contains);
            _selectionElements.Clear();

            var position = new Vector2(8f + 308f, 8f + 28f);

            var previewButton = new Element {
                Position = position,
                Size = new Vector2(150f, 150f),
                Animation = new ActiveAnimation(animation) {
                    IsLooping = true,
                },
            };
            _selectionElements.Add(previewButton);

            var loopToggle = new Element {
                Position = position + new Vector2(0f, 152f),
                Size = new Vector2(150f, 20f),
                Label = "Loop",
                OnClick = () => previewButton.Animation.IsLooping = !previewButton.Animation.IsLooping,
                IsHighlighted = () => previewButton.Animation.IsLooping,
            };
            _selectionElements.Add(loopToggle);

            var playButton = new Element {
                Position = position + new Vector2(0f, 152f + 22f),
                Size = new Vector2(150f, 20f),
                Label = "Play",
                OnClick = previewButton.Animation.Reset,
            };
            _selectionElements.Add(playButton);

            position.X += 152f;

            for (int i = 0; i < animation.Frames.Count; i++) {
                int frameIndex = i;
                var frame = animation.Frames[i];

                var frameButton = new Element {
                    Position = position,
                    Size = new Vector2(42f, 42f),
                    Image = frame.Texture,
                };
                frameButton.OnClick = () => contentBrowserShower.ShowTextureBrowser(
                    texture => {
                        if (texture is null) {
                            animation.Frames.RemoveAt(frameIndex);
                            SelectAnimation(animation, animationFile);
                            return;
                        }

                        animation.Frames[frameIndex].Texture = texture;
                        SelectAnimation(animation, animationFile);
                    });
                _selectionElements.Add(frameButton);

                Vector2 localPosition = position + new Vector2(44f, 0f);

                AddLabelElement(
                    localPosition,
                    "Texture",
                    () => frame.Texture?.GetContentName() ?? "");

                localPosition.Y += 22f;

                AddIncrementalElement(
                    localPosition,
                    "Duration",
                    () => frame.Duration.ToString("N2"),
                    () => frame.Duration += 0.01f,
                    () => frame.Duration -= 0.01f);

                position.Y += 44f;
            }

            position.X += 44f;

            _selectionElements.Add(new Element {
                Position = position,
                Size = new Vector2(300f, 20f),
                Label = "Add Frame",
                OnClick = () => {
                    if (_selectedAnimation is null) {
                        return;
                    }

                    animation.Frames.Add(new Frame {
                        Texture = null,
                        Duration = 0.1f,
                    });

                    SelectAnimation(animation, animationFile);
                },
            });

            position.Y += 28f;

            _selectionElements.Add(new Element {
                Position = position,
                Size = new Vector2(300f, 20f),
                Label = "Save Changes",
                OnClick = () => {
                    if (_selectedAnimation is null) {
                        return;
                    }

                    using (FileStream stream = File.Create(_selectedAnimation)) {
                        JsonSerializer.Serialize(stream, animation, _jsonSerializerOptions);
                    }

                    FileOpener.CopyContentToExecutingDirectory(engineEnvironment, animationFile);
                },
            });

            world.Elements.AddRange(_selectionElements);
        }

        private void AddLabelElement(Vector2 position, string name, Func<string> getValue) {
            _selectionElements.Add(new Element {
                Position = position,
                Size = new Vector2(300f, 20f),
                Label = $"{getValue()}",
            });
        }

        private void AddIncrementalElement(Vector2 position, string name, Func<string> getValue, Action increment, Action decrement) {
            var label = new Element {
                Position = position,
                Size = new Vector2(300f, 20f),
                Label = $"{name}: {getValue()}",
            };
            _selectionElements.Add(label);

            _selectionElements.Add(new Element {
                Position = position + new Vector2(302f, 0f),
                Size = new Vector2(40f, 20f),
                Label = "-10",
                OnClick = () => {
                    for (int i = 0; i < 10; i++) {
                        decrement();
                    }
                    label.Label = $"{name}: {getValue()}";
                },
            });

            _selectionElements.Add(new Element {
                Position = position + new Vector2(302f + 42f, 0f),
                Size = new Vector2(20f, 20f),
                Label = "-",
                OnClick = () => {
                    decrement();
                    label.Label = $"{name}: {getValue()}";
                },
            });

            _selectionElements.Add(new Element {
                Position = position + new Vector2(302f + 42f + 22f, 0f),
                Size = new Vector2(20f, 20f),
                Label = "+",
                OnClick = () => {
                    increment();
                    label.Label = $"{name}: {getValue()}";
                },
            });

            _selectionElements.Add(new Element {
                Position = position + new Vector2(302f + 42f + 22f * 2f, 0f),
                Size = new Vector2(40f, 20f),
                Label = "+10",
                OnClick = () => {
                    for (int i = 0; i < 10; i++) {
                        increment();
                    }
                    label.Label = $"{name}: {getValue()}";
                },
            });
        }
    }
}
