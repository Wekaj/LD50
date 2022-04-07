using LD50.Entities;
using LD50.Graphics;
using LD50.Input;
using LD50.Levels;
using LD50.Scenarios;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LD50.Screens {
    public class GameScreen : IScreen {
        private readonly AnimationManager _animations;
        private readonly SpriteBatch _spriteBatch;
        private readonly InputBindings _bindings;

        private readonly Texture2D _pixelTexture;
        private readonly Texture2D _circleTexture;
        private readonly Texture2D _gunnerTexture;
        private readonly Texture2D _batterTexture;
        private readonly SpriteFont _font;

        private readonly Random _random = new();

        private readonly World _world = new();

        private readonly List<Entity> _drawingEntities = new();

        private readonly List<Scenario> _scenarios = new();

        public GameScreen(ContentManager content, AnimationManager animations, SpriteBatch spriteBatch, InputBindings bindings) {
            _animations = animations;
            _spriteBatch = spriteBatch;
            _bindings = bindings;

            _pixelTexture = content.Load<Texture2D>("Textures/pixel");
            _circleTexture = content.Load<Texture2D>("Textures/circle");
            _gunnerTexture = content.Load<Texture2D>("Textures/Gunner Test 1");
            _batterTexture = content.Load<Texture2D>("Textures/Batter Test 1");
            _font = content.Load<SpriteFont>("Fonts/font");
            
            for (int i = 0; i < 4; i++) {
                var level = new Level();

                int units = _random.Next(2, 10);
                for (int j = 0; j < units; j++) {
                    level.Entities.Add(CreateUnit() with {
                        Team = Team.Player,
                    });
                }

                int enemies = _random.Next(2, 4);
                for (int j = 0; j < enemies; j++) {
                    level.Entities.Add(CreateUnit() with {
                        Team = Team.Enemy,
                        Color = Color.Red,
                    });
                }

                _world.Levels.Add(level);
            }

            _world.Levels[0].Name = "Family Restaurant";
            _world.Levels[1].Name = "Back Alleys";
            _world.Levels[2].Name = "Workrooms";
            _world.Levels[3].Name = "Headquarters";

            _world.CurrentLevel = _world.Levels[0];

            _scenarios.Add(new Scenario {
                Description = "There is a dude and he says \"Hi.\"",
                Choices = {
                    new Choice {
                        Label = "Punch him.",
                        Action = world => {
                            world.PlayerMoney += 1000000;

                            world.CurrentScenario = new Scenario {
                                Description = "You punch the dude and it turns out he bleeds money. You and the boys\nbeat him up and now you have a million dollars.",

                                Choices = {
                                    new Choice {
                                        Label = "Give the money to charity.",
                                        Action = world => {
                                            world.PlayerMoney -= 1000000;
                                            
                                            world.CurrentLevel?.Entities.Add(CreateUnit() with {
                                                Team = Team.Enemy,
                                                Color = Color.Red,

                                                Scale = new Vector2(3f),
                                                
                                                MaxHealth = 1000,
                                                Health = 1000,
                                                AttackDamage = 50,
                                            });

                                            world.CurrentScenario = new Scenario {
                                                Description = "Uh oh, the representative for the charity absorbs all the money\ninstead and powers up. Prepare for combat.",
                                            };
                                        },
                                    },
                                    new Choice {
                                        Label = "Keep the money.",
                                    },
                                }
                            };
                        },
                    },
                    new Choice {
                        Label = "Say hi back.",
                        Action = world => {
                            for (int i = 0; i < 50; i++) {
                                world.CurrentLevel?.Entities.Add(CreateUnit() with {
                                    Team = Team.Enemy,
                                    Color = Color.Red,
                                });
                            }

                            world.CurrentScenario = new Scenario {
                                Description = "Now you've gone and done it. The dude goes mental and\nhe splits into 50 dudes. They're all after you.",
                            };
                        },
                    }
                },
            });

            Scenario guyScenario = null;
            guyScenario = new Scenario {
                Description = "A guy comes up to you and asks to join your gang for $100.\nThe crowd of guys behind him watch with intrigue.",
                Choices = {
                    new Choice {
                        Label = "Give him the money.",
                        Action = world => {
                            if (world.PlayerMoney < 100) {
                                world.CurrentScenario = new Scenario {
                                    Description = "It turns out you don't even have $100. He scoffs and walks away.",
                                };
                                return;
                            }
                            
                            world.PlayerMoney -= 100;

                            Entity recruit = CreateUnit() with {
                                Team = Team.Player,
                            };
                            world.CurrentLevel?.Entities.Add(recruit);

                            world.CurrentScenario = new Scenario {
                                Description = "He thanks you and mentions that one of his friends will be interested in joining too.",
                                Choices = {
                                    new Choice {
                                        Label = "Give the recruit the customary slap on the butt and tell him to be on his way.",
                                        Action = world => {
                                            recruit.Health -= 10;

                                            world.CurrentScenario = guyScenario;
                                        }
                                    },
                                    new Choice {
                                        Label = "Grunt. You don't like this recruit.",
                                        Action = world => {
                                            world.CurrentScenario = guyScenario;
                                        }
                                    },
                                }
                            };
                        },
                    },
                    new Choice {
                        Label = "Reject him.",
                        Action = world => {
                            world.CurrentScenario = new Scenario {
                                Description = "Booing and expletives erupt from the crowd of guys and they all walk away.",
                            };
                        },
                    },
                },
            };
            _scenarios.Add(guyScenario);
        }

        public void Update(GameTime gameTime) {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_world.CurrentScenario is not null) {
                for (int i = 0; i < 4; i++) {
                    if (_bindings.JustPressed(BindingId.Level1 + i)) {
                        if (_world.CurrentScenario.Choices.Count == 0) {
                            _world.CurrentScenario = null;
                            break;
                        }
                        else if (i < _world.CurrentScenario.Choices.Count) {
                            Choice choice = _world.CurrentScenario.Choices[i];
                            _world.CurrentScenario = null;
                            choice.Action(_world);
                            break;
                        }
                    }
                }
            }
            else {
                for (int i = 0; i < 4; i++) {
                    if (_bindings.JustPressed(BindingId.Level1 + i)) {
                        _world.CurrentLevel = _world.Levels[i];
                    }
                }
                
                for (int i = 0; i < _world.Levels.Count; i++) {
                    UpdateLevel(_world.Levels[i], deltaTime);
                }

                _world.ScenarioTimer += deltaTime;
                if (_world.ScenarioTimer >= 20f) {
                    _world.CurrentScenario = _scenarios[_random.Next(_scenarios.Count)];
                    _world.ScenarioTimer = 0f;
                }
            }
        }

        public void Draw(GameTime gameTime) {
            _spriteBatch.Begin();

            if (_world.CurrentLevel is not null) {
                for (int i = 0; i < _world.CurrentLevel.Entities.Count; i++) {
                    DrawEntityShadow(_world.CurrentLevel.Entities[i]);
                }

                _drawingEntities.AddRange(_world.CurrentLevel.Entities.OrderBy(entity => entity.Position.Y));
                for (int i = 0; i < _drawingEntities.Count; i++) {
                    DrawEntity(_drawingEntities[i]);
                }
                _drawingEntities.Clear();

                for (int i = 0; i < _world.CurrentLevel.Entities.Count; i++) {
                    DrawEntityOverlay(_world.CurrentLevel.Entities[i]);
                }
            }

            for (int i = 0; i < _world.Levels.Count; i++) {
                Level level = _world.Levels[i];

                _spriteBatch.DrawString(_font, level.Name, new Vector2(8f + 160f * i, 8f), level == _world.CurrentLevel ? Color.White : Color.Black);
            }

            string moneyString = $"${_world.PlayerMoney}";
            _spriteBatch.DrawString(_font, moneyString, new Vector2(8f, 600f - 8f - _font.MeasureString(moneyString).Y), Color.Black);

            if (_world.CurrentScenario is not null) {
                string text = _world.CurrentScenario.Description;
                
                if (_world.CurrentScenario.Choices.Count > 0) {
                    text += "\n";
                }

                for (int i = 0; i < _world.CurrentScenario.Choices.Count; i++) {
                    text += $"\n{i + 1}: {_world.CurrentScenario.Choices[i].Label}";
                }

                Vector2 textSize = _font.MeasureString(text);
                _spriteBatch.Draw(
                    _pixelTexture,
                    new Vector2(400f - textSize.X / 2f - 8f, 300f - textSize.Y / 2f - 8f),
                    null,
                    Color.Black * 0.5f,
                    0f,
                    Vector2.Zero,
                    new Vector2(textSize.X + 16f, textSize.Y + 16f),
                    SpriteEffects.None,
                    0f);
                _spriteBatch.DrawString(_font, text, Vector2.Floor(new Vector2(400f - textSize.X / 2f, 300f - textSize.Y / 2f)), Color.White);
            }

            _spriteBatch.End();
        }

        private Entity CreateUnit() {
            return _random.Next(2) == 0
                ? CreateGunner()
                : CreateBatter();
        }

        private Entity CreateGunner() {
            return new Entity {
                Position = new Vector2(_random.Next(0, 800), _random.Next(0, 600)),
                Friction = 500f,

                Texture = _gunnerTexture,
                Origin = new Vector2(_gunnerTexture.Width / 2, _gunnerTexture.Height),
                Scale = new Vector2(0.75f),

                DefaultTexture = _gunnerTexture,

                MaxHealth = 80,
                Health = 80,

                AttackRange = 150f,
                AttackDamage = 30,
                AttackCooldown = 2f,

                AttackingAnimation = _animations.GunnerAttacking,
            };
        }

        private Entity CreateBatter() {
            return new Entity {
                Position = new Vector2(_random.Next(0, 800), _random.Next(0, 600)),
                Friction = 500f,

                Texture = _batterTexture,
                Origin = new Vector2(_batterTexture.Width / 2, _batterTexture.Height),
                Scale = new Vector2(0.75f),

                DefaultTexture = _batterTexture,

                MaxHealth = 100,
                Health = 100,

                AttackRange = 50f,
                AttackDamage = 10,
                AttackCooldown = 1f,

                AttackingAnimation = _animations.BatterAttacking,
            };
        }

        private void UpdateLevel(Level level, float deltaTime) {
            for (int i = 0; i < level.Entities.Count; i++) {
                Entity entity = level.Entities[i];

                if (entity.Health <= 0) {
                    level.Entities.RemoveAt(i);
                    i--;

                    if (entity.Team == Team.Enemy) {
                        _world.PlayerMoney += 50;
                    }
                }
            }

            for (int i = 0; i < level.Entities.Count; i++) {
                UpdateEntity(level.Entities[i], level, deltaTime);

                for (int j = i + 1; j < level.Entities.Count; j++) {
                    DoEntityCollisions(level.Entities[i], level.Entities[j]);
                }
            }
        }

        private void UpdateEntity(Entity entity, Level level, float deltaTime) {
            if (entity.Animation is not null) {
                entity.Animation.Update(deltaTime);

                if (entity.Animation.IsFinished) {
                    entity.Animation = null;
                    entity.Texture = entity.DefaultTexture;
                }
                else {
                    entity.Animation.Apply(entity);
                }
            }

            if (entity.TargetEntity is not null && entity.TargetEntity.Health <= 0) {
                entity.TargetEntity = null;
            }

            if (entity.CooldownTimer > 0f) {
                entity.CooldownTimer -= deltaTime;
            }

            if (entity.PreviousHealth < entity.Health) {
                entity.PreviousHealth = entity.Health;
                entity.PreviousHealthTimer = 0f;
            }
            else if (entity.PreviousHealth > entity.Health) {
                entity.PreviousHealthTimer += deltaTime;

                if (entity.PreviousHealthTimer > 0.5f) {
                    entity.PreviousHealth -= 200f * deltaTime;
                }
            }
            else {
                entity.PreviousHealthTimer = 0f;
            }

            if (entity.TargetEntity is null) {
                for (int j = 0; j < level.Entities.Count; j++) {
                    Entity other = level.Entities[j];

                    if (other.Team == entity.Team || Vector2.DistanceSquared(entity.Position, other.Position) > 200f * 200f) {
                        continue;
                    }

                    entity.TargetEntity = other;
                }
            }
            else if (Vector2.DistanceSquared(entity.Position, entity.TargetEntity.Position) <= entity.AttackRange * entity.AttackRange) {
                if (entity.CooldownTimer <= 0f) {
                    entity.CooldownTimer = entity.AttackCooldown;

                    entity.TargetEntity.Health -= entity.AttackDamage;
                    entity.TargetEntity.PreviousHealthTimer = 0f;
                    entity.TargetEntity.CooldownTimer += 0.25f;

                    if (entity.AttackingAnimation is not null) {
                        entity.Animation = entity.AttackingAnimation.Play();
                    }
                }
            }

            if (_random.Next(1000) == 0) {
                entity.TargetPosition = new Vector2(_random.Next(0, 800), _random.Next(0, 600));
            }

            Vector2? targetPosition = entity.TargetEntity?.Position ?? entity.TargetPosition;
            float? targetDistance = entity.TargetEntity is not null ? entity.AttackRange : null;

            if (targetPosition.HasValue && entity.Animation is null) {
                float distance = Vector2.Distance(entity.Position, targetPosition.Value);
                float walkSpeed = 100f * deltaTime;
                if (distance < walkSpeed) {
                    entity.Position = targetPosition.Value;

                    entity.WalkTimer = 0f;
                }
                else if (targetDistance is null || distance > targetDistance.Value) {
                    entity.Position += (targetPosition.Value - entity.Position) * (walkSpeed / distance);

                    if (targetPosition.Value.X > entity.Position.X) {
                        entity.Effects = SpriteEffects.None;
                    }
                    else {
                        entity.Effects = SpriteEffects.FlipHorizontally;
                    }

                    entity.WalkTimer += deltaTime;
                }
                else {
                    entity.WalkTimer = 0f;
                }
            }
            else {
                entity.WalkTimer = 0f;
            }

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

        private void DoEntityCollisions(Entity entity1, Entity entity2) {
            const float entityDiameter = 30f;

            Vector2 delta = entity2.Position - entity1.Position;
            float distance = delta.Length();
            
            if (distance >= entityDiameter) {
                return;
            }
            
            float overlap = entityDiameter - distance;
            Vector2 normal = delta / distance;

            entity1.Force -= normal * overlap * 50f;
            entity2.Force += normal * overlap * 50f;
        }

        private void DrawEntity(Entity entity) {
            if (entity.Texture is null) {
                return;
            }

            _spriteBatch.Draw(
                entity.Texture,
                entity.Position + new Vector2(0f, -(float)Math.Abs(Math.Sin(entity.WalkTimer * 15f)) * 10f),
                null,
                entity.Color,
                (float)Math.Sin(entity.WalkTimer * 15f) * 0.05f,
                entity.Origin,
                entity.Scale,
                entity.Effects,
                0f);
        }

        private void DrawEntityShadow(Entity entity) {
            _spriteBatch.Draw(
                _circleTexture,
                entity.Position,
                null,
                Color.Black * 0.25f,
                0f,
                new Vector2(_circleTexture.Width / 2f, _circleTexture.Height / 2f),
                new Vector2(0.5f, 0.25f),
                SpriteEffects.None,
                0f);
        }

        private void DrawEntityOverlay(Entity entity) {
            if (entity.Health >= entity.MaxHealth) {
                return;
            }

            const float healthBarWidth = 40f;
            const float healthBarHeight = 2f;
            
            Vector2 healthBarPosition = entity.Position + new Vector2(-healthBarWidth / 2f, -70f);

            _spriteBatch.Draw(
                _pixelTexture,
                healthBarPosition,
                null,
                Color.Black,
                0f,
                Vector2.Zero,
                new Vector2(healthBarWidth, healthBarHeight),
                SpriteEffects.None,
                0f);

            _spriteBatch.Draw(
                _pixelTexture,
                healthBarPosition,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                new Vector2(healthBarWidth * entity.PreviousHealth / entity.MaxHealth, healthBarHeight),
                SpriteEffects.None,
                0f);

            _spriteBatch.Draw(
                _pixelTexture,
                healthBarPosition,
                null,
                Color.Red,
                0f,
                Vector2.Zero,
                new Vector2(healthBarWidth * entity.Health / entity.MaxHealth, healthBarHeight),
                SpriteEffects.None,
                0f);
        }
    }
}
