using LD50.Entities;
using LD50.Input;
using LD50.Interface;
using LD50.Levels;
using LD50.Scenarios;
using LD50.Skills;
using Microsoft.Xna.Framework;
using System;

namespace LD50.Screens {
    public class GameScreen(
        World world,
        UnitFactory unitFactory,
        ScenarioShower scenarioShower,
        CommanderSelector commanderSelector)
        : IScreen {

        private readonly Random _random = new();

        private readonly string[] _levelNames = [
            "Family Restaurant",
            "Workrooms",
            "Headquarters",
            "Back Alleys"
        ];

        private readonly Skill _bloodSpikes = new() {
            IsValidTarget = target => target.Team == Team.Player,
            Use = target => {
                const float radius = 100f;

                // Reduce the unit's health and damage all nearby units.
                target.Health -= 30;

                if (world.CurrentLevel is not null) {
                    for (int i = 0; i < world.CurrentLevel.Units.Count; i++) {
                        Unit unit = world.CurrentLevel.Units[i];

                        if (unit.Team != Team.Player && Vector2.DistanceSquared(unit.Entity.Position, target.Entity.Position) < radius * radius) {
                            unit.Health -= 60;
                        }
                    }
                }
            },
        };

        public void Show() {
            const int levels = 4;

            float screenButtonWidth = (800f - 8f * (levels + 1)) / levels;
            for (int i = 0; i < levels; i++) {
                int x = i % 2;
                int y = i / 2;

                var level = new Level {
                    Name = _levelNames[i],
                    Position = new Vector2(x * GameProperties.ScreenWidth, y * GameProperties.ScreenHeight),
                };

                world.Levels.Add(level);

                world.Elements.Add(new Element {
                    Position = new Vector2(8f + (screenButtonWidth + 8f) * x, 8f + (28f * y)),
                    Size = new Vector2(screenButtonWidth, 20f),
                    Label = level.Name,
                    OnClick = () => world.CurrentLevel = level,
                    IsHighlighted = () => world.CurrentLevel == level,
                    Binding = BindingId.Level1 + i,
                });
            }

            Unit[] commanders = [
                unitFactory.CreateCommander(@"Units\lieutenant_minigun.json"),
                unitFactory.CreateCommander(@"Units\lieutenant_dagger.json"),
                unitFactory.CreateCommander(@"Units\lieutenant_molotov.json"),
            ];

            const float commanderButtonWidth = 120f;
            const float commanderButtonHeight = 120f;
            for (int i = 0; i < commanders.Length; i++) {
                Unit commander = commanders[i];

                world.Commanders.Add(commander);
                world.Levels[0].Units.Add(commander);

                if (world.SelectedCommander is null) {
                    commanderSelector.SelectCommander(commander);
                }

                world.Elements.Add(new Element {
                    Position = new Vector2(GameProperties.ScreenWidth - 8f - commanderButtonWidth, 8f + (commanderButtonHeight + 8f) * i),
                    Size = new Vector2(commanderButtonWidth, commanderButtonHeight),
                    Label = commander.Name,
                    Image = commander.Portrait,
                    OnClick = () => {
                        if (world.SelectedCommander == commander) {
                            world.CurrentLevel = commander.CurrentLevel;
                        }
                        else {
                            commanderSelector.SelectCommander(commander);
                        }
                    },
                    IsHighlighted = () => world.SelectedCommander == commander,
                    Binding = BindingId.Commander1 + i,
                });
            }

            world.Levels[0].SpawnPositions.Add(new Vector2(GameProperties.ScreenWidth / 2f, -30f));
            world.Levels[0].SpawnPositions.Add(new Vector2(-30f, GameProperties.ScreenHeight / 2f));

            world.CurrentLevel = world.Levels[0];

            const float elementWidth = 150f;

            world.Elements.Add(new Element {
                Position = new Vector2(8f + (elementWidth + 8f) * 2f, 600f - 8f - 50f),
                Size = new Vector2(elementWidth, 50f),
                Label = "Blood Spikes",
                IsHighlighted = () => world.CurrentSkill == _bloodSpikes,
                OnClick = () => {
                    if (world.CurrentSkill == _bloodSpikes) {
                        world.CurrentSkill = null;
                    }
                    else {
                        world.CurrentSkill = _bloodSpikes;
                    }
                },
                Binding = BindingId.Action3,
            });

            scenarioShower.ShowScenario(new Scenario {
                Description = "The cold storage room nips at your fingers, the family of rats are your only company. You wonder where they could be, its been over 2 hours now.",
                Action = world => {
                    scenarioShower.ShowScenario(new Scenario {
                        Description = "Right on cue, three distinct characters saunter into the room.",
                        Action = world => {
                            scenarioShower.ShowScenario(new Scenario {
                                Description = "'Yo boss, sorry we're late,' the largest of the three groans.",
                                Action = world => {
                                    scenarioShower.ShowScenario(new Scenario {
                                        Description = "You nod in acknowledgement and invite them to gather around you.",
                                        Action = world => {
                                            scenarioShower.ShowScenario(new Scenario {
                                                Description = "Pirro crushes a rat under his boot, laughing manically.",
                                                Action = world => {
                                                    scenarioShower.ShowScenario(new Scenario {
                                                        Description = "Marissa sighs in disapproval. 'Someone put that nutjob on a leash... he almost blew up the bar on the way over here.'",
                                                        Action = world => {
                                                            scenarioShower.ShowScenario(new Scenario {
                                                                Description = "She is interrupted by Alphonso. 'Boss, I'll be straight with ya, we're running low on goods and we gotta do something.'",
                                                                Action = world => {
                                                                    scenarioShower.ShowScenario(new Scenario {
                                                                        Description = "You pause. You've been painfully aware of this issue for some time now.",
                                                                        Action = world => {
                                                                            scenarioShower.ShowScenario(new Scenario {
                                                                                Description = "*BAAAANG*",
                                                                                Action = world => {
                                                                                    scenarioShower.ShowScenario(new Scenario {
                                                                                        Description = "A foot soldier comes crashing through the storage door. 'We got 2 coppas pokin round out front boss!'",
                                                                                        Action = world => {
                                                                                            scenarioShower.ShowScenario(new Scenario {
                                                                                                Description = "Pirro says he'll handle it, marching out the door... Moments later, you hear gunshots.",
                                                                                            });
                                                                                        },
                                                                                    });
                                                                                },
                                                                            });
                                                                        },
                                                                    });
                                                                },
                                                            });
                                                        },
                                                    });
                                                },
                                            });
                                        },
                                    });
                                },
                            });
                        },
                    });
                },
            });
        }

        public void Hide() {
            world.Reset();
        }
    }
}
