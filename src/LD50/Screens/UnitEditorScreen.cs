using LD50.Development;
using LD50.Entities;
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
    public class UnitEditorScreen(
        World world,
        ScreenChanger screenChanger,
        EngineEnvironment engineEnvironment,
        AnimationManager animationManager,
        ContentBrowserShower contentBrowserShower)
        : IScreen {

        private readonly List<Element> _selectionElements = [];

        private readonly JsonSerializerOptions _jsonSerializerOptions = new() {
            WriteIndented = true,
        };

        private string? _selectedUnit;
        
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
                Label = "Unit Editor",
            });

            string[] unitFiles = Directory.GetFiles(engineEnvironment.UnitDirectory, "*.json");

            for (int i = 0; i < unitFiles.Length; i++) {
                string unitFile = unitFiles[i];
                string unitName = Path.ChangeExtension(Path.GetRelativePath(engineEnvironment.UnitDirectory, unitFile), null);

                world.Elements.Add(new Element {
                    Position = new Vector2(8f, 8f + 28f + 22f * i),
                    Size = new Vector2(300f, 20f),
                    Label = unitName,
                    OnClick = () => SelectUnit(unitFile),
                    IsHighlighted = () => _selectedUnit == unitFile,
                });
            }
        }

        public void Hide() {
            world.Reset();

            _selectedUnit = null;
        }

        private void SelectUnit(string unitFile) {
            world.Elements.RemoveWhere(_selectionElements.Contains);
            _selectionElements.Clear();

            _selectedUnit = unitFile;

            UnitProfile unitProfile;
            using (FileStream stream = File.OpenRead(unitFile)) {
                unitProfile = JsonSerializer.Deserialize<UnitProfile>(stream, _jsonSerializerOptions)!;
            }

            var position = new Vector2(8f + 308f, 8f + 28f);

            var textureButton = new Element {
                Position = position,
                Size = new Vector2(120f, 120f),
                Image = unitProfile.Texture,
                Label = "Idle",
            };
            textureButton.OnClick = () => contentBrowserShower.ShowTextureBrowser(
                texture => {
                    unitProfile.Texture = texture;
                    textureButton.Image = texture;
                });
            _selectionElements.Add(textureButton);

            var attackButton = new Element {
                Position = position + new Vector2(122f, 0f),
                Size = new Vector2(120f, 120f),
                Label = "Attack",
            };
            if (unitProfile.AttackingAnimation is not null) {
                attackButton.Animation = new ActiveAnimation(animationManager.GetAnimation(unitProfile.AttackingAnimation)) {
                    IsLooping = true,
                };
            }
            attackButton.OnClick = () => contentBrowserShower.ShowAnimationBrowser(
                animation => {
                    unitProfile.AttackingAnimation = animation;

                    if (animation is not null) {
                        attackButton.Animation = new ActiveAnimation(animationManager.GetAnimation(animation)) {
                            IsLooping = true,
                        };
                    }
                });
            _selectionElements.Add(attackButton);

            var portraitButton = new Element {
                Position = position + new Vector2(122f * 2f, 0f),
                Size = new Vector2(120f, 120f),
                Image = unitProfile.Portrait,
                Label = "Portrait",
            };
            portraitButton.OnClick = () => contentBrowserShower.ShowTextureBrowser(
                texture => {
                    unitProfile.Portrait = texture;
                    portraitButton.Image = texture;
                });
            _selectionElements.Add(portraitButton);

            position.Y += 122f;

            AddLabelElement(
                position,
                "Name",
                () => unitProfile.Name ?? "");

            position.Y += 22f;

            AddIncrementalElement(
                position,
                "Cost",
                () => unitProfile.Cost.ToString(),
                () => unitProfile.Cost++,
                () => unitProfile.Cost--);

            position.Y += 22f;

            AddValueElement(
                position,
                "Minion 1",
                () => unitProfile.Minion1?.GetContentName() ?? "",
                callback => contentBrowserShower.ShowUnitBrowser(unit => {
                    unitProfile.Minion1 = unit;
                    callback();
                }));

            position.Y += 22f;

            AddValueElement(
                position,
                "Minion 2",
                () => unitProfile.Minion2?.GetContentName() ?? "",
                callback => contentBrowserShower.ShowUnitBrowser(unit => {
                    unitProfile.Minion2 = unit;
                    callback();
                }));

            position.Y += 22f;

            AddIncrementalElement(
                position,
                "Mass",
                () => unitProfile.Mass.ToString(),
                () => unitProfile.Mass++,
                () => unitProfile.Mass--);

            position.Y += 22f;

            AddIncrementalElement(
                position,
                "Health",
                () => unitProfile.Health.ToString(),
                () => unitProfile.Health++,
                () => unitProfile.Health--);

            position.Y += 22f;

            AddIncrementalElement(
                position,
                "Vision Range",
                () => unitProfile.VisionRange.ToString(),
                () => unitProfile.VisionRange++,
                () => unitProfile.VisionRange--);

            position.Y += 22f;

            AddIncrementalElement(
                position,
                "Attack Range",
                () => unitProfile.VisionRange.ToString(),
                () => unitProfile.VisionRange++,
                () => unitProfile.VisionRange--);

            position.Y += 22f;

            AddIncrementalElement(
                position,
                "Attack Damage",
                () => unitProfile.AttackDamage.ToString(),
                () => unitProfile.AttackDamage++,
                () => unitProfile.AttackDamage--);

            position.Y += 22f;

            AddIncrementalElement(
                position,
                "Attack Stun",
                () => unitProfile.AttackStun.ToString("N2"),
                () => unitProfile.AttackStun += 0.01f,
                () => unitProfile.AttackStun -= 0.01f);

            position.Y += 22f;

            AddIncrementalElement(
                position,
                "Attack Ticks",
                () => unitProfile.AttackTicks.ToString(),
                () => unitProfile.AttackTicks++,
                () => unitProfile.AttackTicks--);

            position.Y += 22f;

            AddIncrementalElement(
                position,
                "Attack Cooldown",
                () => unitProfile.AttackCooldown.ToString("N1"),
                () => unitProfile.AttackCooldown += 0.1f,
                () => unitProfile.AttackCooldown -= 0.1f);

            position.Y += 22f;

            AddToggleElement(
                position,
                "Throws Molotovs",
                () => unitProfile.ThrowsMolotovs.ToString(),
                () => unitProfile.ThrowsMolotovs = !unitProfile.ThrowsMolotovs);

            position.Y += 22f;

            AddToggleElement(
                position,
                "Formation",
                () => unitProfile.Formation.ToString(),
                () => unitProfile.Formation = (Formation)(((int)unitProfile.Formation + 1) % Enum.GetValues<Formation>().Length));

            position.Y += 28f;

            _selectionElements.Add(new Element {
                Position = position,
                Size = new Vector2(300f, 20f),
                Label = "Save Changes",
                OnClick = () => {
                    using (FileStream stream = File.Create(unitFile)) {
                        JsonSerializer.Serialize(stream, unitProfile, _jsonSerializerOptions);
                    }

                    File.Copy(unitFile, @$"Units\{Path.GetFileName(unitFile)}", true);
                },
            });

            world.Elements.AddRange(_selectionElements);
        }

        private void AddLabelElement(Vector2 position, string name, Func<string> getValue) {
            _selectionElements.Add(new Element {
                Position = position,
                Size = new Vector2(300f, 20f),
                Label = $"{name}: {getValue()}",
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

        private void AddValueElement(Vector2 position, string name, Func<string> getValue, Action<Action> changeValue) {
            var label = new Element {
                Position = position,
                Size = new Vector2(300f, 20f),
                Label = $"{name}:",
            };
            _selectionElements.Add(label);

            var value = new Element {
                Position = position + new Vector2(302f, 0f),
                Size = new Vector2(300f, 20f),
                Label = getValue(),
            };
            value.OnClick = () => {
                changeValue(() => {
                    value.Label = getValue();
                });
            };
            _selectionElements.Add(value);
        }

        private void AddToggleElement(Vector2 position, string name, Func<string> getValue, Action toggle) {
            var button = new Element {
                Position = position,
                Size = new Vector2(300f, 20f),
                Label = $"{name}: {getValue()}",
            };
            button.OnClick = () => {
                toggle();
                button.Label = $"{name}: {getValue()}";
            };
            _selectionElements.Add(button);
        }
    }
}
