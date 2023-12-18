using LD50.Development;
using LD50.Entities;
using LD50.Input;
using LD50.Interface;
using LD50.Scenarios;
using LD50.Screens;
using LD50.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace LD50.Levels {
    public class WorldUpdater(
        World world,
        XnaMouse mouse,
        InputBindings inputBindings,
        InterfaceActions interfaceActions,
        ScenarioShower scenarioShower,
        UnitFactory unitFactory,
        EngineEnvironment engineEnvironment,
        ScreenChanger screenChanger,
        CommanderSelector commanderSelector,
        IDeltaTimeSource deltaTimeSource)
        : IStartupHandler, IFixedUpdateable {

        private readonly Random _random = new();

        private readonly List<Scenario> _scenarios = [];

        public void OnStartup() {
            Scenario guyScenario = null;
            guyScenario = new Scenario {
                Description = "A guy comes up to you and asks to join your gang for $100. The crowd of guys behind him watch with intrigue.",
                Choices = {
                    new Choice {
                        Label = "Give him the money.",
                        Action = world => {
                            if (world.PlayerMoney < 100) {
                                scenarioShower.ShowScenario(new Scenario {
                                    Description = "It turns out you don't even have $100. He scoffs and walks away.",
                                });
                                return;
                            }

                            world.PlayerMoney -= 100;

                            Unit recruit = unitFactory.CreateUnit(world.CurrentLevel) with {
                                Team = Team.Player,
                            };
                            world.CurrentLevel.Units.Add(recruit);

                            scenarioShower.ShowScenario(new Scenario {
                                Description = "He thanks you and mentions that one of his friends will be interested in joining too.",
                                Choices = {
                                    new Choice {
                                        Label = "High five.",
                                        Action = world => {
                                            recruit.Health -= 10;

                                            scenarioShower.ShowScenario(guyScenario);
                                        }
                                    },
                                    new Choice {
                                        Label = "Grunt.",
                                        Action = world => {
                                            scenarioShower.ShowScenario(guyScenario);
                                        }
                                    },
                                }
                            });
                        },
                    },
                    new Choice {
                        Label = "Reject him.",
                        Action = world => {
                            scenarioShower.ShowScenario(new Scenario {
                                Description = "Booing and expletives erupt from the crowd of guys and they all walk away.",
                            });
                        },
                    },
                },
            };
            _scenarios.Add(guyScenario);
        }

        public void FixedUpdate() {
            if (engineEnvironment.ProjectDirectory is not null && (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))) {
                screenChanger.ChangeScreen(new ScreenArgs(ScreenType.Engine));
                return;
            }

            if (inputBindings.JustReleased(BindingId.Select)) {
                DoSelection();
            }

            if (world.SelectedCommander is not null && world.CurrentLevel is not null && inputBindings.JustPressed(BindingId.Move)) {
                world.SelectedCommander.TargetPosition = world.CurrentLevel.Position + mouse.Position;
                world.SelectedCommander.TargetUnit = null;
            }

            interfaceActions.Update(world);

            if (world.Levels.Count > 0 && world.CurrentScenario is null) {
                for (int i = 0; i < world.Levels.Count; i++) {
                    UpdateLevel(world.Levels[i], deltaTimeSource.Latest);
                }

                world.ScenarioTimer += deltaTimeSource.Latest;
                if (world.ScenarioTimer >= 120f) {
                    scenarioShower.ShowScenario(_scenarios[_random.Next(_scenarios.Count)]);
                    world.ScenarioTimer = 0f;
                }
            }
        }

        private void DoSelection() {
            if (interfaceActions.HandleMouseClick(world)) {
                return;
            }

            if (world.CurrentSkill is not null && world.CurrentLevel is not null) {
                for (int i = 0; i < world.CurrentLevel.Units.Count; i++) {
                    Unit entity = world.CurrentLevel.Units[i];

                    if (MouseIntersectsUnit(entity) && world.CurrentSkill.IsValidTarget(entity)) {
                        world.CurrentSkill.Use(entity);
                        world.CurrentSkill = null;
                        return;
                    }
                }
            }

            world.CurrentSkill = null;

            for (int i = 0; i < world.Commanders.Count; i++) {
                Unit commander = world.Commanders[i];

                if (MouseIntersectsUnit(commander)) {
                    commanderSelector.SelectCommander(commander);
                    return;
                }
            }
        }

        private void UpdateLevel(Level level, float deltaTime) {
            level.SpawnTimer -= deltaTime;
            if (level.SpawnTimer <= 0f && level.SpawnPositions.Count > 0) {
                Vector2 spawnPosition = level.SpawnPositions[_random.Next(level.SpawnPositions.Count)];

                int enemies = _random.Next(2, 5);
                for (int i = 0; i < enemies; i++) {
                    Unit enemy = unitFactory.CreateUnit(level) with {
                        Team = Team.Enemy,

                        TargetPosition = level.Position + new Vector2(GameProperties.ScreenWidth / 2f, GameProperties.ScreenHeight / 2f),

                        AttackDamage = 5,
                    };
                    enemy.Entity.Color = Color.Red;
                    enemy.Entity.Position = spawnPosition + new Vector2(_random.Next(-50, 51), _random.Next(-50, 51));

                    level.Units.Add(enemy);
                }

                level.SpawnTimer = _random.Next(5, 20);

                if (world.Commanders.Count > 0) {
                    Unit talker = world.Commanders[_random.Next(world.Commanders.Count)];

                    talker.Dialogue = talker.StrongEnemyQuotes[_random.Next(talker.StrongEnemyQuotes.Count)];
                    talker.DialogueTimer = 5f;
                }
            }

            UpdateFields(level, deltaTime);
            UpdateUnits(level, deltaTime);
            UpdateProjectiles(level, deltaTime);
        }

        private void UpdateUnits(Level level, float deltaTime) {
            for (int i = 0; i < level.Units.Count; i++) {
                Unit unit = level.Units[i];

                if (unit.Health <= 0) {
                    level.Units.RemoveAt(i);
                    i--;

                    if (unit.Team == Team.Enemy) {
                        world.PlayerMoney += 50;
                    }

                    continue;
                }

                if (unit.Entity.Position.X < level.Position.X
                    || unit.Entity.Position.Y < level.Position.Y
                    || unit.Entity.Position.X > level.Position.X + GameProperties.ScreenWidth
                    || unit.Entity.Position.Y > level.Position.Y + GameProperties.ScreenHeight) {

                    for (int j = 0; j < world.Levels.Count; j++) {
                        Level otherLevel = world.Levels[j];

                        if (otherLevel == level) {
                            continue;
                        }

                        if (unit.Entity.Position.X >= otherLevel.Position.X
                            && unit.Entity.Position.Y >= otherLevel.Position.Y
                            && unit.Entity.Position.X <= otherLevel.Position.X + GameProperties.ScreenWidth
                            && unit.Entity.Position.Y <= otherLevel.Position.Y + GameProperties.ScreenHeight) {

                            level.Units.RemoveAt(i);
                            i--;

                            otherLevel.Units.Add(unit);

                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < level.Units.Count; i++) {
                UpdateUnit(level.Units[i], level, deltaTime);

                for (int j = i + 1; j < level.Units.Count; j++) {
                    DoUnitCollisions(level.Units[i], level.Units[j]);
                }
            }
        }

        private void UpdateProjectiles(Level level, float deltaTime) {
            for (int i = 0; i < level.Projectiles.Count; i++) {
                Projectile projectile = level.Projectiles[i];

                UpdateProjectile(projectile, level, deltaTime);

                if (projectile.TravelTimer >= projectile.TravelDuration) {
                    level.Projectiles.RemoveAt(i);
                    i--;

                    if (projectile.Field is not null) {
                        projectile.Field.Entity.Position = projectile.Entity.Position;
                        projectile.Field.Source = projectile.Source;

                        level.Fields.Add(projectile.Field);
                    }
                }
            }
        }

        private void UpdateFields(Level level, float deltaTime) {
            for (int i = 0; i < level.Fields.Count; i++) {
                Field field = level.Fields[i];

                UpdateField(field, level, deltaTime);

                if (field.Life <= 0f) {
                    level.Fields.RemoveAt(i);
                    i--;
                }
            }
        }

        private void UpdateUnit(Unit unit, Level level, float deltaTime) {
            unit.CurrentLevel = level;

            unit.Minions.RemoveWhere(minion => minion.Health <= 0);

            if (unit.DialogueTimer > deltaTime) {
                unit.DialogueTimer -= deltaTime;
            }
            else {
                unit.DialogueTimer = 0f;
                unit.Dialogue = null;
            }

            if (unit.Animation is not null) {
                unit.Animation.Update(deltaTime);

                if (unit.Animation.IsFinished) {
                    unit.Animation = null;
                    unit.Entity.Texture = unit.DefaultTexture;
                }
                else {
                    unit.Entity.Texture = unit.Animation.Apply() ?? unit.Entity.Texture;
                }
            }

            if (unit.TargetUnit is not null && unit.TargetUnit.Health <= 0) {
                unit.TargetUnit = null;
            }

            if (unit.CooldownTimer > 0f) {
                unit.CooldownTimer -= deltaTime;
            }

            if (unit.PreviousHealth < unit.Health) {
                unit.PreviousHealth = unit.Health;
                unit.PreviousHealthTimer = 0f;
            }
            else if (unit.PreviousHealth > unit.Health) {
                unit.PreviousHealthTimer += deltaTime;

                if (unit.PreviousHealthTimer > 0.5f) {
                    unit.PreviousHealth -= 200f * deltaTime;
                }
            }
            else {
                unit.PreviousHealthTimer = 0f;
            }

            if (unit.TargetUnit is null) {
                for (int i = 0; i < level.Units.Count; i++) {
                    Unit other = level.Units[i];

                    if (other.Team == unit.Team
                        || Vector2.DistanceSquared(unit.Entity.Position, other.Entity.Position) > unit.VisionRange * unit.VisionRange
                        || (unit.Commander is not null && Vector2.DistanceSquared(unit.Commander.Entity.Position, other.Entity.Position) > (200f + unit.AttackRange) * (200f + unit.AttackRange))) {

                        continue;
                    }

                    unit.TargetUnit = other;
                }
            }

            if (unit.TargetUnit is not null && Vector2.DistanceSquared(unit.Entity.Position, unit.TargetUnit.Entity.Position) <= unit.AttackRange * unit.AttackRange) {
                if (unit.CooldownTimer <= 0f) {
                    unit.CooldownTimer = unit.AttackCooldown;

                    Attack(unit, unit.TargetUnit, level);

                    unit.Entity.Effects = unit.TargetUnit.Entity.Position.X < unit.Entity.Position.X
                        ? SpriteEffects.FlipHorizontally
                        : SpriteEffects.None;

                    if (unit.AttackingAnimation is not null) {
                        unit.Animation = unit.AttackingAnimation.Play();

                        if (unit.AttackTicks > 1) {
                            unit.AttackTickTimer = unit.AttackingAnimation.Duration / (unit.AttackTicks - 1);
                            unit.AttackingUnit = unit.TargetUnit;
                            unit.RemainingTicks = unit.AttackTicks - 1;
                        }
                    }
                }
            }

            if (unit.AttackingUnit is not null && unit.RemainingTicks > 0) {
                unit.AttackTickTimer -= deltaTime;

                if (unit.AttackTickTimer <= 0f) {
                    Attack(unit, unit.AttackingUnit, level);

                    unit.AttackTickTimer += unit.AttackingAnimation.Duration / (unit.AttackTicks - 1);
                    unit.RemainingTicks--;
                }
            }
            else {
                unit.AttackingUnit = null;
            }

            unit.TargetUnit ??= unit.Commander?.TargetUnit;

            if (unit.Commander is not null && unit.TargetUnit != unit.Commander.TargetUnit) {
                float allowedDistance = unit.TargetUnit is not null ? 250f : 150f;

                Vector2 commanderPosition = unit.Commander.TargetPosition ?? unit.Commander.Entity.Position;

                if (Vector2.DistanceSquared(unit.Entity.Position, commanderPosition) > allowedDistance * allowedDistance) {
                    unit.TargetUnit = null;

                    //if (!entity.TargetPosition.HasValue || Vector2.DistanceSquared(entity.TargetPosition.Value, commanderPosition) > allowedDistance * allowedDistance) {
                    //    entity.TargetPosition = commanderPosition + AngleToVector(_random.NextSingle() * MathHelper.TwoPi) * 100f;
                    //}
                }
            }

            if (unit.TargetPosition.HasValue && unit.Entity.Position != unit.TargetPosition) {
                unit.Direction = GetAngle(unit.TargetPosition.Value - unit.Entity.Position);
            }

            if (unit.Minions.Count > 0) {
                int arcMinions = 0;
                int groupMinions = 0;

                for (int i = 0; i < unit.Minions.Count; i++) {
                    Unit minion = unit.Minions[i];

                    switch (minion.Formation) {
                        case Formation.FrontArc:
                            arcMinions++;
                            break;
                        case Formation.Group:
                            groupMinions++;
                            break;
                    }
                }

                Vector2 minionTargetPosition = unit.TargetPosition ?? unit.Entity.Position;

                // Position the arc units in an arc in front of the commander.
                if (arcMinions > 0) {
                    float angle = unit.Direction - MathHelper.PiOver2;
                    float angleStep = MathHelper.Pi / (arcMinions - 1);

                    var arcPositions = new List<Vector2>();
                    for (int i = 0; i < arcMinions; i++) {
                        arcPositions.Add(minionTargetPosition + AngleToVector(angle) * 110f);
                        angle += angleStep;
                    }

                    for (int i = 0; i < unit.Minions.Count; i++) {
                        Unit minion = unit.Minions[i];

                        if (minion.Formation == Formation.FrontArc) {
                            Vector2 bestPosition = arcPositions./*OrderBy(position => Vector2.DistanceSquared(minion.Position, position)).*/First();
                            arcPositions.Remove(bestPosition);

                            minion.TargetPosition = bestPosition;
                        }
                    }
                }

                // Position the group units in a circle around the commander.
                if (groupMinions > 0) {
                    float angle = unit.Direction;
                    float angleStep = MathHelper.Pi * 2f / groupMinions;

                    var groupPositions = new List<Vector2>();
                    for (int i = 0; i < groupMinions; i++) {
                        groupPositions.Add(minionTargetPosition + AngleToVector(angle) * 60f);
                        angle += angleStep;
                    }

                    for (int i = 0; i < unit.Minions.Count; i++) {
                        Unit minion = unit.Minions[i];

                        if (minion.Formation == Formation.Group) {
                            Vector2 bestPosition = groupPositions./*OrderBy(position => Vector2.DistanceSquared(minion.Position, position)).*/First();
                            groupPositions.Remove(bestPosition);

                            minion.TargetPosition = bestPosition;
                        }
                    }
                }
            }

            Vector2? targetPosition = unit.TargetUnit?.Entity.Position ?? unit.TargetPosition;
            float? targetDistance = unit.TargetUnit is not null ? unit.AttackRange : null;

            if (unit.PrioritisesTargetPosition) {
                targetPosition = unit.TargetPosition ?? unit.TargetUnit?.Entity.Position;
                targetDistance = unit.TargetPosition is null ? unit.AttackRange : null;
            }

            if (targetPosition.HasValue /*&& entity.Animation is null*/) {
                float speedModifier = unit.Animation is null ? 1f : 0.75f;

                float distance = Vector2.Distance(unit.Entity.Position, targetPosition.Value);
                float walkSpeed = 100f * speedModifier * deltaTime;

                if (distance < walkSpeed) {
                    unit.Entity.Position = targetPosition.Value;

                    unit.WalkTimer = 0f;
                }
                else if (targetDistance is null || distance > targetDistance.Value) {
                    unit.Entity.Position += (targetPosition.Value - unit.Entity.Position) * (walkSpeed / distance);

                    if (targetPosition.Value.X > unit.Entity.Position.X) {
                        unit.Entity.Effects = SpriteEffects.None;
                    }
                    else {
                        unit.Entity.Effects = SpriteEffects.FlipHorizontally;
                    }

                    unit.WalkTimer += deltaTime * speedModifier;
                }
                else {
                    unit.WalkTimer = 0f;
                }
            }
            else {
                unit.WalkTimer = 0f;
            }

            if (unit.AttackingUnit is not null) {
                unit.Entity.Effects = unit.AttackingUnit.Entity.Position.X < unit.Entity.Position.X
                    ? SpriteEffects.FlipHorizontally
                    : SpriteEffects.None;
            }

            unit.Entity.Depth = (float)Math.Abs(Math.Sin(unit.WalkTimer * 15f)) * 10f;
            unit.Entity.Rotation = (float)Math.Sin(unit.WalkTimer * 15f) * 0.05f;

            UpdateEntity(unit.Entity, deltaTime);
        }

        private void Attack(Unit attacker, Unit target, Level level) {
            target.Health -= attacker.AttackDamage;
            target.PreviousHealthTimer = 0f;
            target.CooldownTimer += attacker.AttackStun;

            if (attacker.ThrowsMolotovs) {
                level.Projectiles.Add(CreateMolotov(
                    attacker.Entity.Position,
                    target.Entity.Position
                        + target.Entity.Velocity * 1f // TODO: make leading actually work (unit velocity usually isn't set)
                        + AngleToVector(_random.NextSingle() * MathHelper.TwoPi) * _random.NextSingle() * 100f,
                    attacker));
            }
        }

        private Projectile CreateMolotov(Vector2 start, Vector2 end, Unit? source = null) {
            return new Projectile {
                Entity = {
                    Texture = "Textures/Molo",
                    Origin = new Vector2(0.5f),
                },

                Source = source,

                Start = start,
                End = end,
                Peak = 100f,
                TravelDuration = 1f,

                RotationSpeed = -5f + _random.NextSingle() * 10f,
                Field = new Field {
                    Life = 2f,
                    Radius = 100f,
                    DamagePerTick = 10,
                    TickInterval = 2f / 5f,
                },
            };
        }

        private void UpdateProjectile(Projectile projectile, Level level, float deltaTime) {
            projectile.TravelTimer += deltaTime;

            float p = projectile.TravelTimer / projectile.TravelDuration;

            projectile.Entity.Position = Vector2.Lerp(projectile.Start, projectile.End, p);
            projectile.Entity.Depth = projectile.Peak * (float)Math.Sin(MathHelper.Pi * p);

            projectile.Entity.Rotation += projectile.RotationSpeed * deltaTime;

            UpdateEntity(projectile.Entity, deltaTime);
        }

        private void UpdateField(Field field, Level level, float deltaTime) {
            field.TickTimer += deltaTime;
            if (field.TickTimer >= field.TickInterval) {
                field.TickTimer -= field.TickInterval;

                for (int i = 0; i < level.Units.Count; i++) {
                    Unit unit = level.Units[i];

                    if (unit.Team == field.Source?.Team
                        || Vector2.DistanceSquared(unit.Entity.Position, field.Entity.Position) > field.Radius * field.Radius) {

                        continue;
                    }

                    unit.Health -= field.DamagePerTick;
                }
            }

            field.Life -= deltaTime;

            UpdateEntity(field.Entity, deltaTime);
        }

        private static void UpdateEntity(Entity entity, float deltaTime) {
            // Apply force.
            entity.Velocity += (entity.Impulse + entity.Force * deltaTime) / entity.Mass;
            entity.Impulse = Vector2.Zero;
            entity.Force = Vector2.Zero;

            // Apply friction.
            float speed = entity.Velocity.Length();
            if (speed > entity.Friction * deltaTime) {
                entity.Velocity *= 1f - entity.Friction * deltaTime / speed;
            }
            else {
                entity.Velocity = Vector2.Zero;
            }

            // Apply velocity.
            entity.Position += entity.Velocity * deltaTime;
        }

        private void DoUnitCollisions(Unit unit1, Unit unit2) {
            if (unit1.Team == unit2.Team) {
                return;
            }

            const float entityDiameter = 50f;

            Vector2 delta = unit2.Entity.Position - unit1.Entity.Position;
            float distance = delta.Length();

            if (distance >= entityDiameter) {
                return;
            }

            float overlap = entityDiameter - distance;
            Vector2 normal = delta / distance;

            unit1.Entity.Force -= normal * overlap * 50f;
            unit2.Entity.Force += normal * overlap * 50f;
        }

        private bool MouseIntersectsUnit(Unit unit) {
            if (world.CurrentLevel is null) {
                return false;
            }

            Vector2 worldMousePosition = world.CurrentLevel.Position + mouse.Position;

            return Vector2.DistanceSquared(worldMousePosition, unit.Entity.Position - new Vector2(0f, 30f)) < 30f * 30f;
        }

        private static float GetAngle(Vector2 vector) {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        private Vector2 AngleToVector(float angle) {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
    }
}
