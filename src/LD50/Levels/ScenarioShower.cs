using LD50.Content;
using LD50.Interface;
using LD50.Scenarios;
using LD50.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LD50.Levels {
    public class ScenarioShower(
        World world,
        IContentManager content) {

        public void ShowScenario(Scenario scenario) {
            var font = content.Load<SpriteFont>("Fonts/font");

            world.CurrentScenario = scenario;

            Vector2 descriptionSize = font.MeasureString(scenario.Description.WrapText(font, 490f));
            var descriptionPosition = new Vector2(400f - 250f, 300f - descriptionSize.Y / 2f);

            Action? onClick = null;
            if (scenario.Choices.Count == 0 || scenario.Action is not null) {
                onClick = () => {
                    HideScenario();
                    scenario.Action?.Invoke(world);
                };
            }

            world.ScenarioElements.Add(new Element {
                Position = descriptionPosition,
                Size = new Vector2(500f, descriptionSize.Y + 10f),
                Label = scenario.Description,
                IsTextBlock = true,
                Margin = 5f,
                OnClick = onClick,
            });

            for (int i = 0; i < scenario.Choices.Count; i++) {
                Choice choice = scenario.Choices[i];

                world.ScenarioElements.Add(new Element {
                    Position = descriptionPosition + new Vector2(0f, descriptionSize.Y + 10f + 8f + 28f * i),
                    Size = new Vector2(500f, 20f),
                    Label = choice.Label,
                    OnClick = () => {
                        HideScenario();
                        choice.Action(world);
                    },
                });
            }

            world.Elements.AddRange(world.ScenarioElements);
        }

        private void HideScenario() {
            world.CurrentScenario = null;

            world.Elements.RemoveAll(world.ScenarioElements.Contains);
            world.ScenarioElements.Clear();
        }
    }
}
