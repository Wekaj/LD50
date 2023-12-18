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
using System.Reflection.Emit;
using System.Text.Json;
using System.Xml.Linq;

namespace LD50.Screens {
    public class UnitEditorScreen(
        World world,
        ScreenChanger screenChanger,
        EngineEnvironment engineEnvironment,
        AnimationManager animationManager)
        : IScreen {

        private readonly List<Element> _selectionElements = [];

        private readonly JsonSerializerOptions _jsonSerializerOptions = new() {
            WriteIndented = true,
        };

        private string? _selectedUnit;
        
        public void Show() {
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

            string unitDirectory = Path.Combine(engineEnvironment.ProjectDirectory ?? "", @"res\units\");
            string[] unitFiles = Directory.GetFiles(unitDirectory, "*.json");

            for (int i = 0; i < unitFiles.Length; i++) {
                string unitFile = unitFiles[i];
                string unitName = Path.GetFileNameWithoutExtension(unitFile);

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

            _selectionElements.Add(new Element {
                Position = position,
                Size = new Vector2(120f, 120f),
                Image = unitProfile.Texture,
            });

            _selectionElements.Add(new Element {
                Position = position + new Vector2(122f, 0f),
                Size = new Vector2(120f, 120f),
                Animation = new ActiveAnimation(animationManager.Animations[unitProfile.AttackingAnimation]) {
                    IsLooping = true,
                },
            });

            position.Y += 122f;

            AddIncrementalElement(
                position,
                "Cost",
                () => unitProfile.Cost.ToString(),
                () => unitProfile.Cost++,
                () => unitProfile.Cost--);

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
                "Attack Damage",
                () => unitProfile.AttackDamage.ToString(),
                () => unitProfile.AttackDamage++,
                () => unitProfile.AttackDamage--);

            position.Y += 22f;

            AddIncrementalElement(
                position,
                "Attack Ticks",
                () => unitProfile.AttackTicks.ToString(),
                () => unitProfile.AttackTicks++,
                () => unitProfile.AttackTicks--);

            position.Y += 22f;

            AddToggleElement(
                position,
                "Throws Molotovs",
                () => unitProfile.ThrowsMolotovs.ToString(),
                () => unitProfile.ThrowsMolotovs = !unitProfile.ThrowsMolotovs);

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

        private void AddIncrementalElement(Vector2 position, string name, Func<string> getValue, Action increment, Action decrement) {
            var label = new Element {
                Position = position,
                Size = new Vector2(300f, 20f),
                Label = $"{name}: {getValue()}",
            };
            _selectionElements.Add(label);

            _selectionElements.Add(new Element {
                Position = position + new Vector2(302f, 0f),
                Size = new Vector2(20f, 20f),
                Label = "-",
                OnClick = () => {
                    decrement();
                    label.Label = $"{name}: {getValue()}";
                },
            });

            _selectionElements.Add(new Element {
                Position = position + new Vector2(302f + 22f, 0f),
                Size = new Vector2(20f, 20f),
                Label = "+",
                OnClick = () => {
                    increment();
                    label.Label = $"{name}: {getValue()}";
                },
            });
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
