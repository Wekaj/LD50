using LD50.Entities;
using LD50.Graphics;
using LD50.Input;
using LD50.Interface;
using LD50.Levels;
using LD50.Scenarios;
using LD50.Skills;
using LD50.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LD50.Screens {
    public class GameScreen : IScreen {
        private const float _levelWidth = 800f;
        private const float _levelHeight = 600f;

        private readonly AnimationManager _animations;
        private readonly SpriteBatch _spriteBatch;
        private readonly InputBindings _bindings;
        private readonly XnaMouse _mouse;
        private readonly InterfaceActions _interfaceActions;

        private readonly Texture2D _pixelTexture;
        private readonly Texture2D _circleTexture;
        private readonly Texture2D _gunnerTexture;
        private readonly Texture2D _batterTexture;
        private readonly Texture2D _minigunLieutenantTexture;
        private readonly Texture2D _daggerLieutenantTexture;
        private readonly Texture2D _portraitAlphonsoTexture;
        private readonly Texture2D _portraitMarissaTexture;
        private readonly SpriteFont _font;

        private readonly Random _random = new();

        private readonly World _world = new();

        private readonly List<Entity> _drawingEntities = new();

        private readonly List<Scenario> _scenarios = new();

        private readonly string[] _levelNames = new[] {
            "Family Restaurant",
            "Workrooms",
            "Headquarters",
            "Back Alleys"
        };

        private readonly Skill _bloodSpikes;
        private Skill? _currentSkill;

        public GameScreen(
            ContentManager content,
            AnimationManager animations,
            SpriteBatch spriteBatch,
            InputBindings bindings,
            XnaMouse mouse,
            InterfaceActions interfaceActions) {

            _animations = animations;
            _spriteBatch = spriteBatch;
            _bindings = bindings;
            _mouse = mouse;
            _interfaceActions = interfaceActions;
            
            _pixelTexture = content.Load<Texture2D>("Textures/pixel");
            _circleTexture = content.Load<Texture2D>("Textures/circle");
            _gunnerTexture = content.Load<Texture2D>("Textures/Gunner Test 1");
            _batterTexture = content.Load<Texture2D>("Textures/Batter Test 1");
            _minigunLieutenantTexture = content.Load<Texture2D>("Textures/Lieutenant Test 1");
            _daggerLieutenantTexture = content.Load<Texture2D>("Textures/Lieutenant2 Test 1");
            _portraitAlphonsoTexture = content.Load<Texture2D>("Textures/portrait_alphonso");
            _portraitMarissaTexture = content.Load<Texture2D>("Textures/portrait_marissa");
            _font = content.Load<SpriteFont>("Fonts/font");

            const int levels = 4;
            
            float screenButtonWidth = (800f - 8f * (levels + 1)) / levels;
            for (int i = 0; i < levels; i++) {
                int x = i % 2;
                int y = i / 2;

                var level = new Level {
                    Name = _levelNames[i],
                    Position = new Vector2(x * _levelWidth, y * _levelHeight),
                };

                _world.Levels.Add(level);

                _world.Elements.Add(new Element {
                    Position = new Vector2(8f + (screenButtonWidth + 8f) * x, 8f + (28f * y)),
                    Size = new Vector2(screenButtonWidth, 20f),
                    Label = level.Name,
                    OnClick = () => _world.CurrentLevel = level,
                    IsHighlighted = () => _world.CurrentLevel == level,
                });
            }

            Entity[] commanders = new[] {
                CreateMinigunLieutenant(),
                CreateDaggerLieutenant(),
            };

            const float commanderButtonWidth = 100f;
            const float commanderButtonHeight = 60f;
            for (int i = 0; i < commanders.Length; i++) {
                Entity commander = commanders[i];

                _world.Commanders.Add(commander);
                _world.Levels[0].Entities.Add(commander);

                _world.SelectedCommander ??= commander;

                _world.Elements.Add(new Element {
                    Position = new Vector2(_levelWidth - 8f - commanderButtonWidth, 8f + (commanderButtonHeight + 8f) * i),
                    Size = new Vector2(commanderButtonWidth, commanderButtonHeight),
                    Label = commander.Name,
                    Image = commander.Portrait,
                    OnClick = () => _world.SelectedCommander = commander,
                    IsHighlighted = () => _world.SelectedCommander == commander,
                });
            }

            _world.Levels[0].SpawnPositions.Add(new Vector2(_levelWidth / 2f, -30f));
            _world.Levels[0].SpawnPositions.Add(new Vector2(-30f, _levelHeight / 2f));

            _world.CurrentLevel = _world.Levels[0];

            _bloodSpikes = new Skill {
                IsValidTarget = target => target.Team == Team.Player,
                Use = target => {
                    const float radius = 100f;

                    // Reduce the unit's health and damage all nearby units.
                    target.Health -= 30;

                    for (int i = 0; i < _world.CurrentLevel.Entities.Count; i++) {
                        Entity entity = _world.CurrentLevel.Entities[i];

                        if (entity.Team != Team.Player && Vector2.DistanceSquared(entity.Position, target.Position) < radius * radius) {
                            entity.Health -= 60;
                        }
                    }
                },
            };

            _world.Elements.Add(new Element {
                Position = new Vector2(8f, 600f - 8f - 50f),
                Size = new Vector2(100f, 50f),
                Label = "Buy Batter\nCost: $50",
                OnClick = () => {
                    if (_world.SelectedCommander is null || _world.PlayerMoney < 50) {
                        return;
                    }

                    Entity entity = CreateBatter() with {
                        Position = _world.SelectedCommander.Position + AngleToVector(_random.NextSingle() * MathHelper.TwoPi) * 50f,
                        Team = Team.Player,
                        Commander = _world.SelectedCommander,
                    };

                    _world.PlayerMoney -= 50;
                    _world.CurrentLevel.Entities.Add(entity);
                    _world.SelectedCommander.Minions.Add(entity);
                },
            });
            _world.Elements.Add(new Element {
                Position = new Vector2(8f + 100f + 8f, 600f - 8f - 50f),
                Size = new Vector2(100f, 50f),
                Label = "Buy Gunner\nCost: $100",
                OnClick = () => {
                    if (_world.SelectedCommander is null || _world.PlayerMoney < 100) {
                        return;
                    }

                    Entity entity = CreateGunner() with {
                        Position = _world.SelectedCommander.Position + AngleToVector(_random.NextSingle() * MathHelper.TwoPi) * 50f,
                        Team = Team.Player,
                        Commander = _world.SelectedCommander,
                    };

                    _world.PlayerMoney -= 100;
                    _world.CurrentLevel.Entities.Add(entity);
                    _world.SelectedCommander.Minions.Add(entity);
                },
            });
            _world.Elements.Add(new Element {
                Position = new Vector2(8f + (100f + 8f) * 2f, 600f - 8f - 50f),
                Size = new Vector2(100f, 50f),
                Label = "Blood Spikes",
                IsHighlighted = () => _currentSkill == _bloodSpikes,
                OnClick = () => {
                    if (_currentSkill == _bloodSpikes) {
                        _currentSkill = null;
                    }
                    else {
                        _currentSkill = _bloodSpikes;
                    }
                },
            });

            ShowScenario(new Scenario {
                Description = "The cold storage room nips at your fingers, the family of rats are your only company. You wonder where they could be, its been over 2 hours now.",
                Action = world => {
                    ShowScenario(new Scenario {
                        Description = "Right on cue, three distinct characters saunter into the room.",
                        Action = world => {
                            ShowScenario(new Scenario {
                                Description = "'Yo boss, sorry we're late,' the largest of the three groans.",
                                Action = world => {
                                    ShowScenario(new Scenario {
                                        Description = "You nod in acknowledgement and invite them to gather around you.",
                                        Action = world => {
                                            ShowScenario(new Scenario {
                                                Description = "Pirro crushes a rat under his boot, laughing manically.",
                                                Action = world => {
                                                    ShowScenario(new Scenario {
                                                        Description = "Marissa sighs in disapproval. 'Someone put that nutjob on a leash... he almost blew up the bar on the way over here.'",
                                                        Action = world => {
                                                            ShowScenario(new Scenario {
                                                                Description = "She is interrupted by Alphonso. 'Boss, I'll be straight with ya, we're running low on goods and we gotta do something.'",
                                                                Action = world => {
                                                                    ShowScenario(new Scenario {
                                                                        Description = "You pause. You've been painfully aware of this issue for some time now.",
                                                                        Action = world => {
                                                                            ShowScenario(new Scenario {
                                                                                Description = "*BAAAANG*",
                                                                                Action = world => {
                                                                                    ShowScenario(new Scenario {
                                                                                        Description = "A foot soldier comes crashing through the storage door. 'We got 2 coppas pokin round out front boss!'",
                                                                                        Action = world => {
                                                                                            ShowScenario(new Scenario {
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

            _scenarios.Add(new Scenario {
                Description = "There is a dude and he says \"Hi.\"",
                Choices = {
                    new Choice {
                        Label = "Punch him.",
                        Action = world => {
                            world.PlayerMoney += 1000000;

                            ShowScenario(new Scenario {
                                Description = "You punch the dude and it turns out he bleeds money. You and the boys beat him up and now you have a million dollars.",

                                Choices = {
                                    new Choice {
                                        Label = "Give the money to charity.",
                                        Action = world => {
                                            world.PlayerMoney -= 1000000;
                                            
                                            world.CurrentLevel?.Entities.Add(CreateUnit(world.CurrentLevel) with {
                                                Team = Team.Enemy,
                                                Color = Color.Red,

                                                Scale = new Vector2(3f),
                                                
                                                MaxHealth = 1000,
                                                Health = 1000,
                                                AttackDamage = 50,
                                            });

                                            ShowScenario(new Scenario {
                                                Description = "Uh oh, the representative for the charity absorbs all the money instead and powers up. Prepare for combat.",
                                            });
                                        },
                                    },
                                    new Choice {
                                        Label = "Keep the money.",
                                    },
                                }
                            });
                        },
                    },
                    new Choice {
                        Label = "Say hi back.",
                        Action = world => {
                            for (int i = 0; i < 50; i++) {
                                world.CurrentLevel?.Entities.Add(CreateUnit(world.CurrentLevel) with {
                                    Team = Team.Enemy,
                                    Color = Color.Red,
                                });
                            }

                            ShowScenario(new Scenario {
                                Description = "Now you've gone and done it. The dude goes mental and he splits into 50 dudes. They're all after you.",
                            });
                        },
                    }
                },
            });

            Scenario guyScenario = null;
            guyScenario = new Scenario {
                Description = "A guy comes up to you and asks to join your gang for $100. The crowd of guys behind him watch with intrigue.",
                Choices = {
                    new Choice {
                        Label = "Give him the money.",
                        Action = world => {
                            if (world.PlayerMoney < 100) {
                                ShowScenario(new Scenario {
                                    Description = "It turns out you don't even have $100. He scoffs and walks away.",
                                });
                                return;
                            }
                            
                            world.PlayerMoney -= 100;

                            Entity recruit = CreateUnit(world.CurrentLevel) with {
                                Team = Team.Player,
                            };
                            world.CurrentLevel.Entities.Add(recruit);

                            ShowScenario(new Scenario {
                                Description = "He thanks you and mentions that one of his friends will be interested in joining too.",
                                Choices = {
                                    new Choice {
                                        Label = "High five.",
                                        Action = world => {
                                            recruit.Health -= 10;

                                            ShowScenario(guyScenario);
                                        }
                                    },
                                    new Choice {
                                        Label = "Grunt.",
                                        Action = world => {
                                            ShowScenario(guyScenario);
                                        }
                                    },
                                }
                            });
                        },
                    },
                    new Choice {
                        Label = "Reject him.",
                        Action = world => {
                            ShowScenario(new Scenario {
                                Description = "Booing and expletives erupt from the crowd of guys and they all walk away.",
                            });
                        },
                    },
                },
            };
            _scenarios.Add(guyScenario);
        }

        public void Update(GameTime gameTime) {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_bindings.JustReleased(BindingId.Select)) {
                DoSelection();
            }

            if (_world.SelectedCommander is not null && _world.CurrentLevel is not null && _bindings.JustPressed(BindingId.Move)) {
                _world.SelectedCommander.TargetPosition = _world.CurrentLevel.Position + _mouse.Position;
                _world.SelectedCommander.TargetEntity = null;
            }

            if (_world.CurrentScenario is null) {
                for (int i = 0; i < _world.Levels.Count; i++) {
                    UpdateLevel(_world.Levels[i], deltaTime);
                }

                _world.ScenarioTimer += deltaTime;
                if (_world.ScenarioTimer >= 120f) {
                    ShowScenario(_scenarios[_random.Next(_scenarios.Count)]);
                    _world.ScenarioTimer = 0f;
                }
            }
        }

        private void DoSelection() {
            if (_interfaceActions.HandleMouseClick(_world)) {
                return;
            }

            if (_currentSkill is not null && _world.CurrentLevel is not null) {
                for (int i = 0; i < _world.CurrentLevel.Entities.Count; i++) {
                    Entity entity = _world.CurrentLevel.Entities[i];

                    if (MouseIntersectsEntity(entity) && _currentSkill.IsValidTarget(entity)) {
                        _currentSkill.Use(entity);
                        _currentSkill = null;
                        return;
                    }
                }
            }

            _currentSkill = null;

            for (int i = 0; i < _world.Commanders.Count; i++) {
                Entity commander = _world.Commanders[i];

                if (MouseIntersectsEntity(commander)) {
                    _world.SelectedCommander = commander;
                    return;
                }
            }
        }

        public void Draw(GameTime gameTime) {
            if (_world.CurrentLevel is not null) {
                _spriteBatch.Begin(transformMatrix: Matrix.CreateTranslation(new Vector3(-_world.CurrentLevel.Position, 0f)));

                for (int i = 0; i < _world.Levels.Count; i++) {
                    Level level = _world.Levels[i];

                    for (int j = 0; j < level.Entities.Count; j++) {
                        DrawEntityPath(level.Entities[j]);
                    }
                }

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

                _spriteBatch.End();
            }

            _interfaceActions.DrawInterface(_world);
        }

        private Entity CreateUnit(Level level) {
            Entity unit = _random.Next(2) == 0
                ? CreateGunner()
                : CreateBatter();

            return unit with {
                Position = level.Position + new Vector2(_random.Next(0, 800), _random.Next(0, 600)),
            };
        }

        private Entity CreateGunner() {
            return new Entity {
                Friction = 500f,

                Texture = _gunnerTexture,
                Origin = new Vector2(_gunnerTexture.Width / 2, _gunnerTexture.Height),
                Scale = new Vector2(0.75f),

                DefaultTexture = _gunnerTexture,

                MaxHealth = 80,
                Health = 80,

                AttackRange = 150f,
                AttackDamage = 10,
                AttackStun = 0.025f,
                AttackTicks = 3,
                AttackCooldown = 2f,

                AttackingAnimation = _animations.GunnerAttacking,

                Formation = Formation.Group,
            };
        }

        private Entity CreateBatter() {
            return new Entity {
                Friction = 500f,

                Texture = _batterTexture,
                Origin = new Vector2(_batterTexture.Width / 2, _batterTexture.Height),
                Scale = new Vector2(0.75f),

                DefaultTexture = _batterTexture,

                MaxHealth = 100,
                Health = 100,

                AttackRange = 50f,
                AttackDamage = 10,
                AttackStun = 0.25f,
                AttackCooldown = 1f,

                AttackingAnimation = _animations.BatterAttacking,

                Formation = Formation.FrontArc,
            };
        }

        private Entity CreateMinigunLieutenant() {
            return new Entity {
                Name = "Alphonso",
                Portrait = _portraitAlphonsoTexture,

                StrongEnemyQuotes = {
                    "Everyone, prepare yourself!",
                    "We need all the firepower we can get for this one.",
                    "This is gonna hurt.",
                },

                Position = new Vector2(_random.Next(0, 800), _random.Next(0, 600)),
                Friction = 500f,
                Mass = 5f,

                PrioritisesTargetPosition = true,

                Texture = _minigunLieutenantTexture,
                Origin = new Vector2(_minigunLieutenantTexture.Width / 2, _minigunLieutenantTexture.Height),
                Scale = new Vector2(0.75f),

                DefaultTexture = _minigunLieutenantTexture,

                MaxHealth = 300,
                Health = 300,

                AttackRange = 200f,
                AttackDamage = 15,
                AttackStun = 0.025f,
                AttackTicks = 5,
                AttackCooldown = 3f,

                AttackingAnimation = _animations.MinigunLieutenantAttacking,

                DrawPath = true,
            };
        }

        private Entity CreateDaggerLieutenant() {
            return new Entity {
                Name = "Marissa",
                Portrait = _portraitMarissaTexture,

                StrongEnemyQuotes = {
                    "No time for breaks, huh...",
                    "This won't be easy.",
                    "I'll need an extra sharp blade for this one...",
                },

                Position = new Vector2(_random.Next(0, 800), _random.Next(0, 600)),
                Friction = 500f,
                Mass = 5f,

                PrioritisesTargetPosition = true,

                Texture = _daggerLieutenantTexture,
                Origin = new Vector2(_daggerLieutenantTexture.Width / 2, _daggerLieutenantTexture.Height),
                Scale = new Vector2(0.75f),

                DefaultTexture = _daggerLieutenantTexture,

                MaxHealth = 200,
                Health = 200,

                AttackRange = 50f,
                AttackDamage = 10,
                AttackStun = 0.025f,
                AttackTicks = 2,
                AttackCooldown = 0.5f,

                DrawPath = true,
            };
        }

        private void UpdateLevel(Level level, float deltaTime) {
            level.SpawnTimer -= deltaTime;
            if (level.SpawnTimer <= 0f && level.SpawnPositions.Count > 0) {
                Vector2 spawnPosition = level.SpawnPositions[_random.Next(level.SpawnPositions.Count)];
                
                int enemies = _random.Next(2, 5);
                for (int i = 0; i < enemies; i++) {
                    level.Entities.Add(CreateUnit(level) with {
                        Team = Team.Enemy,
                        Color = Color.Red,

                        Position = spawnPosition + new Vector2(_random.Next(-10, 11), _random.Next(-10, 11)),
                        TargetPosition = level.Position + new Vector2(_levelWidth / 2f, _levelHeight / 2f),

                        AttackDamage = 5,
                    });
                }

                level.SpawnTimer = _random.Next(5, 20);

                if (_world.Commanders.Count > 0) {
                    Entity talker = _world.Commanders[_random.Next(_world.Commanders.Count)];

                    talker.Dialogue = talker.StrongEnemyQuotes[_random.Next(talker.StrongEnemyQuotes.Count)];
                    talker.DialogueTimer = 3f;
                }
            }

            for (int i = 0; i < level.Entities.Count; i++) {
                Entity entity = level.Entities[i];

                if (entity.Health <= 0) {
                    level.Entities.RemoveAt(i);
                    i--;

                    if (entity.Team == Team.Enemy) {
                        _world.PlayerMoney += 50;
                    }

                    continue;
                }

                if (entity.Position.X < level.Position.X
                    || entity.Position.Y < level.Position.Y
                    || entity.Position.X > level.Position.X + _levelWidth
                    || entity.Position.Y > level.Position.Y + _levelHeight) {
                    
                    for (int j = 0; j < _world.Levels.Count; j++) {
                        Level otherLevel = _world.Levels[j];

                        if (otherLevel == level) {
                            continue;
                        }

                        if (entity.Position.X >= otherLevel.Position.X
                            && entity.Position.Y >= otherLevel.Position.Y
                            && entity.Position.X <= otherLevel.Position.X + _levelWidth
                            && entity.Position.Y <= otherLevel.Position.Y + _levelHeight) {

                            level.Entities.RemoveAt(i);
                            i--;

                            otherLevel.Entities.Add(entity);

                            break;
                        }
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
            entity.Minions.RemoveWhere(minion => minion.Health <= 0);

            if (entity.DialogueTimer > deltaTime) {
                entity.DialogueTimer -= deltaTime;
            }
            else {
                entity.DialogueTimer = 0f;
                entity.Dialogue = null;
            }

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
                for (int i = 0; i < level.Entities.Count; i++) {
                    Entity other = level.Entities[i];

                    if (other.Team == entity.Team || Vector2.DistanceSquared(entity.Position, other.Position) > 200f * 200f) {
                        continue;
                    }

                    entity.TargetEntity = other;
                }
            }

            if (entity.TargetEntity is not null && Vector2.DistanceSquared(entity.Position, entity.TargetEntity.Position) <= entity.AttackRange * entity.AttackRange) {
                if (entity.CooldownTimer <= 0f) {
                    entity.CooldownTimer = entity.AttackCooldown;

                    entity.TargetEntity.Health -= entity.AttackDamage;
                    entity.TargetEntity.PreviousHealthTimer = 0f;
                    entity.TargetEntity.CooldownTimer += entity.AttackStun;

                    entity.Effects = entity.TargetEntity.Position.X < entity.Position.X
                        ? SpriteEffects.FlipHorizontally
                        : SpriteEffects.None;

                    if (entity.AttackingAnimation is not null) {
                        entity.Animation = entity.AttackingAnimation.Play();

                        if (entity.AttackTicks > 1) {
                            entity.AttackTickTimer = entity.AttackingAnimation.Duration / (entity.AttackTicks - 1);
                            entity.AttackingEntity = entity.TargetEntity;
                            entity.RemainingTicks = entity.AttackTicks - 1;
                        }
                    }
                }
            }

            if (entity.AttackingEntity is not null && entity.RemainingTicks > 0) {
                entity.AttackTickTimer -= deltaTime;

                if (entity.AttackTickTimer <= 0f) {
                    entity.AttackingEntity.Health -= entity.AttackDamage;
                    entity.AttackingEntity.PreviousHealthTimer = 0f;
                    entity.AttackingEntity.CooldownTimer += entity.AttackStun;

                    entity.AttackTickTimer += entity.AttackingAnimation.Duration / (entity.AttackTicks - 1);
                    entity.RemainingTicks--;
                }
            }
            else {
                entity.AttackingEntity = null;
            }

            entity.TargetEntity ??= entity.Commander?.TargetEntity;

            if (entity.Commander is not null) {
                float allowedDistance = entity.TargetEntity is not null ? 250f : 150f;

                Vector2 commanderPosition = entity.Commander.TargetPosition ?? entity.Commander.Position;

                if (Vector2.DistanceSquared(entity.Position, commanderPosition) > allowedDistance * allowedDistance) {
                    entity.TargetEntity = null;

                    //if (!entity.TargetPosition.HasValue || Vector2.DistanceSquared(entity.TargetPosition.Value, commanderPosition) > allowedDistance * allowedDistance) {
                    //    entity.TargetPosition = commanderPosition + AngleToVector(_random.NextSingle() * MathHelper.TwoPi) * 100f;
                    //}
                }
            }

            if (entity.TargetPosition.HasValue && entity.Position != entity.TargetPosition) {
                entity.Direction = GetAngle(entity.TargetPosition.Value - entity.Position);
            }

            if (entity.Minions.Count > 0) {
                int arcMinions = 0;
                int groupMinions = 0;
                
                for (int i = 0; i < entity.Minions.Count; i++) {
                    Entity minion = entity.Minions[i];

                    switch (minion.Formation) {
                        case Formation.FrontArc:
                            arcMinions++;
                            break;
                        case Formation.Group:
                            groupMinions++;
                            break;
                    }
                }

                Vector2 minionTargetPosition = entity.TargetPosition ?? entity.Position;

                // Position the arc units in an arc in front of the commander.
                if (arcMinions > 0) {
                    float angle = entity.Direction - MathHelper.PiOver2;
                    float angleStep = MathHelper.Pi / (arcMinions - 1);

                    var arcPositions = new List<Vector2>();
                    for (int i = 0; i < arcMinions; i++) {
                        arcPositions.Add(minionTargetPosition + AngleToVector(angle) * 110f);
                        angle += angleStep;
                    }

                    for (int i = 0; i < entity.Minions.Count; i++) {
                        Entity minion = entity.Minions[i];

                        if (minion.Formation == Formation.FrontArc) {
                            Vector2 bestPosition = arcPositions./*OrderBy(position => Vector2.DistanceSquared(minion.Position, position)).*/First();
                            arcPositions.Remove(bestPosition);

                            minion.TargetPosition = bestPosition;
                        }
                    }
                }

                // Position the group units in a circle around the commander.
                if (groupMinions > 0) {
                    float angle = entity.Direction;
                    float angleStep = MathHelper.Pi * 2f / groupMinions;

                    var groupPositions = new List<Vector2>();
                    for (int i = 0; i < groupMinions; i++) {
                        groupPositions.Add(minionTargetPosition + AngleToVector(angle) * 60f);
                        angle += angleStep;
                    }

                    for (int i = 0; i < entity.Minions.Count; i++) {
                        Entity minion = entity.Minions[i];

                        if (minion.Formation == Formation.Group) {
                            Vector2 bestPosition = groupPositions./*OrderBy(position => Vector2.DistanceSquared(minion.Position, position)).*/First();
                            groupPositions.Remove(bestPosition);

                            minion.TargetPosition = bestPosition;
                        }
                    }
                }
            }

            Vector2? targetPosition = entity.TargetEntity?.Position ?? entity.TargetPosition;
            float? targetDistance = entity.TargetEntity is not null ? entity.AttackRange : null;

            if (entity.PrioritisesTargetPosition) {
                targetPosition = entity.TargetPosition ?? entity.TargetEntity?.Position;
                targetDistance = entity.TargetPosition is null ? entity.AttackRange : null;
            }

            if (targetPosition.HasValue /*&& entity.Animation is null*/) {
                float speedModifier = entity.Animation is null ? 1f : 0.75f;

                float distance = Vector2.Distance(entity.Position, targetPosition.Value);
                float walkSpeed = 100f * speedModifier * deltaTime;
                
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

                    entity.WalkTimer += deltaTime * speedModifier;
                }
                else {
                    entity.WalkTimer = 0f;
                }
            }
            else {
                entity.WalkTimer = 0f;
            }

            if (entity.AttackingEntity is not null) {
                entity.Effects = entity.AttackingEntity.Position.X < entity.Position.X
                    ? SpriteEffects.FlipHorizontally
                    : SpriteEffects.None;
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
            if (entity1.Team == entity2.Team) {
                return;
            }

            const float entityDiameter = 50f;

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

        private void DrawEntityPath(Entity entity) {
            if (!entity.DrawPath || !entity.TargetPosition.HasValue) {
                return;
            }
            
            _spriteBatch.Draw(
                _pixelTexture,
                entity.Position,
                null,
                Color.Black,
                GetAngle(entity.TargetPosition.Value - entity.Position),
                new Vector2(0f, 0.5f),
                new Vector2(Vector2.Distance(entity.Position, entity.TargetPosition.Value), 1f),
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

        private bool MouseIntersectsEntity(Entity entity) {
            if (_world.CurrentLevel is null) {
                return false;
            }

            Vector2 worldMousePosition = _world.CurrentLevel.Position + _mouse.Position;

            return Vector2.DistanceSquared(worldMousePosition, entity.Position - new Vector2(0f, 30f)) < 30f * 30f;
        }

        private void ShowScenario(Scenario scenario) {
            _world.CurrentScenario = scenario;

            Vector2 descriptionSize = _font.MeasureString(scenario.Description.WrapText(_font, 290f));
            Vector2 descriptionPosition = new Vector2(400f - 150f, 300f - descriptionSize.Y / 2f);

            Action? onClick = null;
            if (scenario.Choices.Count == 0 || scenario.Action is not null) {
                onClick = () => {
                    HideScenario();
                    scenario.Action?.Invoke(_world);
                };
            }

            _world.ScenarioElements.Add(new Element {
                Position = descriptionPosition,
                Size = new Vector2(300f, descriptionSize.Y + 10f),
                Label = scenario.Description,
                IsTextBlock = true,
                Margin = 5f,
                OnClick = onClick,
            });

            for (int i = 0; i < scenario.Choices.Count; i++) {
                Choice choice = scenario.Choices[i];

                _world.ScenarioElements.Add(new Element {
                    Position = descriptionPosition + new Vector2(0f, descriptionSize.Y + 10f + 8f + 28f * i),
                    Size = new Vector2(300f, 20f),
                    Label = choice.Label,
                    OnClick = () => {
                        HideScenario();
                        choice.Action(_world);
                    },
                });
            }

            _world.Elements.AddRange(_world.ScenarioElements);
        }

        private void HideScenario() {
            _world.CurrentScenario = null;

            _world.Elements.RemoveAll(_world.ScenarioElements.Contains);
            _world.ScenarioElements.Clear();
        }

        private static float GetAngle(Vector2 vector) {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        private Vector2 AngleToVector(float angle) {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
    }
}
