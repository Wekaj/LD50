using LD50.Entities;
using LD50.Input;
using LD50.Interface;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace LD50.Levels {
    public class CommanderSelector(
        World world,
        UnitFactory unitFactory) {

        private const float _elementWidth = 150f;

        private readonly Random _random = new();

        public void SelectCommander(Unit commander) {
            world.Elements.RemoveAll(world.SelectedCommanderElements.Contains);
            world.SelectedCommanderElements.Clear();

            if (commander.Minion1 is not null) {
                CreateMinionButton(commander.Minion1, new Vector2(8f + (_elementWidth + 8f) * 0f, 600f - 8f - 50f), BindingId.Action1);
            }
            if (commander.Minion2 is not null) {
                CreateMinionButton(commander.Minion2, new Vector2(8f + (_elementWidth + 8f) * 1f, 600f - 8f - 50f), BindingId.Action2);
            }

            world.SelectedCommander = commander;
        }

        private void CreateMinionButton(string minionProfile, Vector2 position, BindingId binding) {
            UnitProfile unitProfile;
            using (FileStream stream = File.OpenRead(minionProfile)) {
                unitProfile = JsonSerializer.Deserialize<UnitProfile>(stream)!;
            }

            var button = new Element {
                Position = position,
                Size = new Vector2(_elementWidth, 50f),
                Label = $"Buy {unitProfile.Name}\nCost: ${unitProfile.Cost}",
                OnClick = () => {
                    if (world.SelectedCommander is null || world.CurrentLevel is null || world.PlayerMoney < unitProfile.Cost) {
                        return;
                    }

                    Unit unit = unitFactory.CreateUnit(minionProfile) with {
                        Team = Team.Player,
                        Commander = world.SelectedCommander,
                    };
                    unit.Entity.Position = world.SelectedCommander.Entity.Position + AngleToVector(_random.NextSingle() * MathHelper.TwoPi) * 50f;

                    world.PlayerMoney -= unitProfile.Cost;
                    world.CurrentLevel.Units.Add(unit);
                    world.SelectedCommander.Minions.Add(unit);
                },
                Binding = binding,
            };

            world.Elements.Add(button);
            world.SelectedCommanderElements.Add(button);
        }

        private Vector2 AngleToVector(float angle) {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
    }
}
