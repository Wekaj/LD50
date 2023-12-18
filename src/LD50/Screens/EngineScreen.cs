﻿using LD50.Development;
using LD50.Interface;
using LD50.Levels;
using Microsoft.Xna.Framework;

namespace LD50.Screens {
    public class EngineScreen(
        EngineEnvironment engineEnvironment,
        World world,
        ScreenChanger screenChanger)
        : IScreen {

        public void Show() {
            world.Elements.Add(new Element {
                Position = new Vector2(GameProperties.ScreenWidth * 0.5f - 100f, 8f),
                Size = new Vector2(200f, 20f),
                Label = $"{GameProperties.Name} Engine",
            });

            world.Elements.Add(new Element {
                Position = new Vector2(8f, 8f + 28f),
                Size = new Vector2(GameProperties.ScreenWidth - 16f, 20f),
                Label = engineEnvironment.ProjectDirectory ?? "",
            });

            world.Elements.Add(new Element {
                Position = new Vector2(GameProperties.ScreenWidth - 308f, 8f + 28f * 2f),
                Size = new Vector2(300f, 50f),
                Label = "Play Game",
                OnClick = () => screenChanger.ChangeScreen(new ScreenArgs(ScreenType.Game)),
            });

            world.Elements.Add(new Element {
                Position = new Vector2(8f, 8f + 28f * 2f),
                Size = new Vector2(300f, 50f),
                Label = "Edit Units",
                OnClick = () => { },
            });

            world.Elements.Add(new Element {
                Position = new Vector2(8f, 8f + 28f * 2f + 58f),
                Size = new Vector2(300f, 50f),
                Label = "Edit Animations",
                OnClick = () => { },
            });

            world.Elements.Add(new Element {
                Position = new Vector2(8f, 8f + 28f * 2f + 58f * 2f),
                Size = new Vector2(300f, 50f),
                Label = "Edit Events",
                OnClick = () => { },
            });
        }

        public void Hide() {
            world.Reset();
        }
    }
}
