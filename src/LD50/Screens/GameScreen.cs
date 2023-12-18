using LD50.Content;
using LD50.Entities;
using LD50.Graphics;
using LD50.Input;
using LD50.Interface;
using LD50.Levels;
using LD50.Scenarios;
using LD50.Skills;
using LD50.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LD50.Screens {
    public class GameScreen : IScreen, IInitializable {
        private const float _levelWidth = 960f;
        private const float _levelHeight = 600f;
        private readonly IContentManager _content;
        private readonly AnimationManager _animations;
        private readonly IGraphicsDeviceSource _graphicsDeviceSource;
        private readonly InputBindings _bindings;
        private readonly XnaMouse _mouse;
        private readonly InterfaceActions _interfaceActions;

        private SpriteBatch _spriteBatch;

        private Texture2D _pixelTexture;
        private Texture2D _circleTexture;
        private SpriteFont _font;

        private readonly Random _random = new();

        private readonly World _world = new();

        private readonly List<Entity> _drawingEntities = [];

        private readonly List<Scenario> _scenarios = [];

        private readonly string[] _levelNames = [
            "Family Restaurant",
            "Workrooms",
            "Headquarters",
            "Back Alleys"
        ];

        private readonly Skill _bloodSpikes;
        private Skill? _currentSkill;

        public GameScreen(
            IContentManager content,
            AnimationManager animations,
            IGraphicsDeviceSource graphicsDeviceSource,
            InputBindings bindings,
            XnaMouse mouse,
            InterfaceActions interfaceActions) {

            _content = content;
            _animations = animations;
            _graphicsDeviceSource = graphicsDeviceSource;
            _bindings = bindings;
            _mouse = mouse;
            _interfaceActions = interfaceActions;

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
                    Binding = BindingId.Level1 + i,
                });
            }

            Unit[] commanders = new[] {
                CreateMinigunLieutenant(),
                CreateDaggerLieutenant(),
                CreateMolotovLieutenant(),
            };

            const float commanderButtonWidth = 120f;
            const float commanderButtonHeight = 120f;
            for (int i = 0; i < commanders.Length; i++) {
                Unit commander = commanders[i];

                _world.Commanders.Add(commander);
                _world.Levels[0].Units.Add(commander);

                _world.SelectedCommander ??= commander;

                _world.Elements.Add(new Element {
                    Position = new Vector2(_levelWidth - 8f - commanderButtonWidth, 8f + (commanderButtonHeight + 8f) * i),
                    Size = new Vector2(commanderButtonWidth, commanderButtonHeight),
                    Label = commander.Name,
                    Image = commander.Portrait,
                    ImageScale = new Vector2(0.5f),
                    OnClick = () => {
                        if (_world.SelectedCommander == commander) {
                            _world.CurrentLevel = commander.CurrentLevel;
                        }
                        else {
                            _world.SelectedCommander = commander;
                        }
                    },
                    IsHighlighted = () => _world.SelectedCommander == commander,
                    Binding = BindingId.Commander1 + i,
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

                    for (int i = 0; i < _world.CurrentLevel.Units.Count; i++) {
                        Unit unit = _world.CurrentLevel.Units[i];

                        if (unit.Team != Team.Player && Vector2.DistanceSquared(unit.Entity.Position, target.Entity.Position) < radius * radius) {
                            unit.Health -= 60;
                        }
                    }
                },
            };

            const float elementWidth = 150f;

            _world.Elements.Add(new Element {
                Position = new Vector2(8f + (elementWidth + 8f) * 0f, 600f - 8f - 50f),
                Size = new Vector2(elementWidth, 50f),
                Label = "Buy Batter\nCost: $50",
                IsVisible = () => _world.SelectedCommander?.Name == "Alphonso",
                OnClick = () => {
                    if (_world.SelectedCommander is null || _world.PlayerMoney < 50) {
                        return;
                    }

                    Unit unit = CreateBatter() with {
                        Team = Team.Player,
                        Commander = _world.SelectedCommander,
                    };
                    unit.Entity.Position = _world.SelectedCommander.Entity.Position + AngleToVector(_random.NextSingle() * MathHelper.TwoPi) * 50f;

                    _world.PlayerMoney -= 50;
                    _world.CurrentLevel.Units.Add(unit);
                    _world.SelectedCommander.Minions.Add(unit);
                },
                Binding = BindingId.Action1,
            });
            _world.Elements.Add(new Element {
                Position = new Vector2(8f + (elementWidth + 8f) * 1f, 600f - 8f - 50f),
                Size = new Vector2(elementWidth, 50f),
                Label = "Buy Gunner\nCost: $100",
                IsVisible = () => _world.SelectedCommander?.Name == "Alphonso",
                OnClick = () => {
                    if (_world.SelectedCommander is null || _world.PlayerMoney < 100) {
                        return;
                    }

                    Unit unit = CreateGunner() with {
                        Team = Team.Player,
                        Commander = _world.SelectedCommander,
                    };
                    unit.Entity.Position = _world.SelectedCommander.Entity.Position + AngleToVector(_random.NextSingle() * MathHelper.TwoPi) * 50f;

                    _world.PlayerMoney -= 100;
                    _world.CurrentLevel.Units.Add(unit);
                    _world.SelectedCommander.Minions.Add(unit);
                },
                Binding = BindingId.Action2,
            });
            _world.Elements.Add(new Element {
                Position = new Vector2(8f + (elementWidth + 8f) * 0f, 600f - 8f - 50f),
                Size = new Vector2(elementWidth, 50f),
                Label = "Buy Pistolier\nCost: $100",
                IsVisible = () => _world.SelectedCommander?.Name == "Marissa",
                OnClick = () => {
                    if (_world.SelectedCommander is null || _world.PlayerMoney < 100) {
                        return;
                    }

                    Unit unit = CreatePistolWoman() with {
                        Team = Team.Player,
                        Commander = _world.SelectedCommander,
                    };
                    unit.Entity.Position = _world.SelectedCommander.Entity.Position + AngleToVector(_random.NextSingle() * MathHelper.TwoPi) * 50f;

                    _world.PlayerMoney -= 100;
                    _world.CurrentLevel.Units.Add(unit);
                    _world.SelectedCommander.Minions.Add(unit);
                },
                Binding = BindingId.Action1,
            });
            _world.Elements.Add(new Element {
                Position = new Vector2(8f + (elementWidth + 8f) * 1f, 600f - 8f - 50f),
                Size = new Vector2(elementWidth, 50f),
                Label = "Buy Rifler\nCost: $125",
                IsVisible = () => _world.SelectedCommander?.Name == "Marissa",
                OnClick = () => {
                    if (_world.SelectedCommander is null || _world.PlayerMoney < 125) {
                        return;
                    }

                    Unit unit = CreateRifleWoman() with {
                        Team = Team.Player,
                        Commander = _world.SelectedCommander,
                    };
                    unit.Entity.Position = _world.SelectedCommander.Entity.Position + AngleToVector(_random.NextSingle() * MathHelper.TwoPi) * 50f;

                    _world.PlayerMoney -= 125;
                    _world.CurrentLevel.Units.Add(unit);
                    _world.SelectedCommander.Minions.Add(unit);
                },
                Binding = BindingId.Action2,
            });
            _world.Elements.Add(new Element {
                Position = new Vector2(8f + (elementWidth + 8f) * 2f, 600f - 8f - 50f),
                Size = new Vector2(elementWidth, 50f),
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
                Binding = BindingId.Action3,
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

                            Unit recruit = CreateUnit(world.CurrentLevel) with {
                                Team = Team.Player,
                            };
                            world.CurrentLevel.Units.Add(recruit);

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

        public void Initialize() {
            _spriteBatch = new SpriteBatch(_graphicsDeviceSource.GraphicsDevice);

            _pixelTexture = _content.Load<Texture2D>("Textures/pixel");
            _circleTexture = _content.Load<Texture2D>("Textures/circle");
            _font = _content.Load<SpriteFont>("Fonts/font");

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
        }

        public void Update(GameTime gameTime) {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_bindings.JustReleased(BindingId.Select)) {
                DoSelection();
            }

            if (_world.SelectedCommander is not null && _world.CurrentLevel is not null && _bindings.JustPressed(BindingId.Move)) {
                _world.SelectedCommander.TargetPosition = _world.CurrentLevel.Position + _mouse.Position;
                _world.SelectedCommander.TargetUnit = null;
            }

            _interfaceActions.Update(_world);

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
                for (int i = 0; i < _world.CurrentLevel.Units.Count; i++) {
                    Unit entity = _world.CurrentLevel.Units[i];

                    if (MouseIntersectsUnit(entity) && _currentSkill.IsValidTarget(entity)) {
                        _currentSkill.Use(entity);
                        _currentSkill = null;
                        return;
                    }
                }
            }

            _currentSkill = null;

            for (int i = 0; i < _world.Commanders.Count; i++) {
                Unit commander = _world.Commanders[i];

                if (MouseIntersectsUnit(commander)) {
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

                    for (int j = 0; j < level.Units.Count; j++) {
                        DrawUnitPath(level.Units[j]);
                    }
                }

                for (int i = 0; i < _world.CurrentLevel.Units.Count; i++) {
                    DrawEntityShadow(_world.CurrentLevel.Units[i].Entity);
                }
                for (int i = 0; i < _world.CurrentLevel.Projectiles.Count; i++) {
                    DrawEntityShadow(_world.CurrentLevel.Projectiles[i].Entity);
                }
                for (int i = 0; i < _world.CurrentLevel.Fields.Count; i++) {
                    DrawField(_world.CurrentLevel.Fields[i]);
                }

                _drawingEntities.AddRange(_world.CurrentLevel.Units
                    .Select(unit => unit.Entity)
                    .Concat(_world.CurrentLevel.Projectiles.Select(projectile => projectile.Entity))
                    .OrderBy(entity => entity.Position.Y));
                for (int i = 0; i < _drawingEntities.Count; i++) {
                    DrawEntity(_drawingEntities[i]);
                }
                _drawingEntities.Clear();

                for (int i = 0; i < _world.CurrentLevel.Units.Count; i++) {
                    DrawUnitOverlay(_world.CurrentLevel.Units[i]);
                }

                _spriteBatch.End();
            }

            _interfaceActions.DrawInterface(_world);
        }

        private Unit CreateUnit(Level level) {
            Unit unit = _random.Next(2) == 0
                ? CreateGunner()
                : CreateBatter();

            unit.Entity.Position = level.Position + new Vector2(_random.Next(0, 800), _random.Next(0, 600));
            
            return unit;
        }

        private Unit CreateGunner() {
            return new Unit {
                Entity = {
                    Friction = 500f,

                    Texture = "Textures/Gunner Test 1",
                    Origin = new Vector2(0.5f, 1f),
                    Scale = new Vector2(0.75f),
                },

                DefaultTexture = "Textures/Gunner Test 1",

                MaxHealth = 80,
                Health = 80,

                VisionRange = 200f,
                AttackRange = 150f,
                AttackDamage = 10,
                AttackStun = 0.025f,
                AttackTicks = 3,
                AttackCooldown = 2f,

                AttackingAnimation = _animations.GunnerAttacking,

                Formation = Formation.Group,
            };
        }

        private Unit CreateBatter() {
            return new Unit {
                Entity = {
                    Friction = 500f,

                    Texture = "Textures/Batter Test 1",
                    Origin = new Vector2(0.5f, 1f),
                    Scale = new Vector2(0.75f),
                },

                DefaultTexture = "Textures/Batter Test 1",

                MaxHealth = 100,
                Health = 100,

                VisionRange = 200f,
                AttackRange = 50f,
                AttackDamage = 10,
                AttackStun = 0.25f,
                AttackCooldown = 1f,

                AttackingAnimation = _animations.BatterAttacking,

                Formation = Formation.FrontArc,
            };
        }

        private Unit CreateRifleWoman() {
            return new Unit {
                Entity = {
                    Friction = 500f,

                    Texture = "Textures/RifleWoman Test 1",
                    Origin = new Vector2(0.5f, 1f),
                    Scale = new Vector2(0.75f),
                },

                DefaultTexture = "Textures/RifleWoman Test 1",

                MaxHealth = 60,
                Health = 60,

                VisionRange = 250f,
                AttackRange = 250f,
                AttackDamage = 60,
                AttackStun = 0.5f,
                AttackCooldown = 3f,

                AttackingAnimation = _animations.RifleWomanAttacking,

                Formation = Formation.Group,
            };
        }

        private Unit CreatePistolWoman() {
            return new Unit {
                Entity = {
                    Friction = 500f,

                    Texture = "Textures/PistolWoman Test 1",
                    Origin = new Vector2(0.5f, 1f),
                    Scale = new Vector2(0.75f),
                },

                DefaultTexture = "Textures/PistolWoman Test 1",

                MaxHealth = 70,
                Health = 70,

                VisionRange = 200f,
                AttackRange = 100f,
                AttackDamage = 3,
                AttackStun = 0.025f,
                AttackTicks = 10,
                AttackCooldown = 2f,

                AttackingAnimation = _animations.PistolWomanAttacking,

                Formation = Formation.FrontArc,
            };
        }

        private Unit CreateMinigunLieutenant() {
            return new Unit {
                Name = "Alphonso",
                Portrait = "Textures/portrait_alphonso",

                StrongEnemyQuotes = {
                    "Everyone, prepare yourself!",
                    "We need all the firepower we can get for this one.",
                    "This is gonna hurt.",
                },

                Entity = {
                    Position = new Vector2(_random.Next(0, 800), _random.Next(0, 600)),
                    Friction = 500f,
                    Mass = 5f,

                    Texture = "Textures/Lieutenant Test 1",
                    Origin = new Vector2(0.5f, 1f),
                    Scale = new Vector2(0.75f),
                },

                PrioritisesTargetPosition = true,

                DefaultTexture = "Textures/Lieutenant Test 1",

                MaxHealth = 300,
                Health = 300,

                VisionRange = 300f,
                AttackRange = 300f,
                AttackDamage = 15,
                AttackStun = 0.025f,
                AttackTicks = 5,
                AttackCooldown = 3f,

                AttackingAnimation = _animations.MinigunLieutenantAttacking,

                DrawPath = true,
            };
        }

        private Unit CreateDaggerLieutenant() {
            return new Unit {
                Name = "Marissa",
                Portrait = "Textures/Lieutenant2_Portrait2 Test 1",

                StrongEnemyQuotes = {
                    "No time for breaks, huh...",
                    "This won't be easy.",
                    "I'll need an extra sharp blade for this one...",
                },

                Entity = {
                    Position = new Vector2(_random.Next(0, 800), _random.Next(0, 600)),
                    Friction = 500f,
                    Mass = 5f,

                    Texture = "Textures/Lieutenant2 Test 1",
                    Origin = new Vector2(0.5f, 1f),
                    Scale = new Vector2(0.75f),
                },

                PrioritisesTargetPosition = true,

                DefaultTexture = "Textures/Lieutenant2 Test 1",

                MaxHealth = 200,
                Health = 200,

                VisionRange = 300f,
                AttackRange = 300f,
                AttackDamage = 20,
                AttackStun = 0.025f,
                AttackCooldown = 0.5f,

                AttackingAnimation = _animations.DaggerLieutenantAttacking,

                DrawPath = true,
            };
        }

        private Unit CreateMolotovLieutenant() {
            return new Unit {
                Name = "Pirro",
                Portrait = "Textures/portrait_pirro",

                StrongEnemyQuotes = {
                    "Uh oh, this can't be good!",
                    "Imma need more bombs for this one!",
                    "It's party time!",
                },
                
                Entity = {
                    Position = new Vector2(_random.Next(0, 800), _random.Next(0, 600)),
                    Friction = 500f,
                    Mass = 5f,

                    Texture = "Textures/Lieutenant3 Test 1",
                    Origin = new Vector2(0.5f, 1f),
                    Scale = new Vector2(0.75f),
                },

                PrioritisesTargetPosition = true,

                DefaultTexture = "Textures/Lieutenant3 Test 1",

                MaxHealth = 250,
                Health = 250,

                VisionRange = 300f,
                AttackRange = 300f,
                AttackDamage = 0,
                AttackStun = 0f,
                AttackTicks = 2,
                AttackCooldown = 4f,
                ThrowsMolotovs = true,

                AttackingAnimation = _animations.MolotovLieutenantAttacking,

                DrawPath = true,
            };
        }

        private void UpdateLevel(Level level, float deltaTime) {
            level.SpawnTimer -= deltaTime;
            if (level.SpawnTimer <= 0f && level.SpawnPositions.Count > 0) {
                Vector2 spawnPosition = level.SpawnPositions[_random.Next(level.SpawnPositions.Count)];

                int enemies = _random.Next(2, 5);
                for (int i = 0; i < enemies; i++) {
                    Unit enemy = CreateUnit(level) with {
                        Team = Team.Enemy,

                        TargetPosition = level.Position + new Vector2(_levelWidth / 2f, _levelHeight / 2f),

                        AttackDamage = 5,
                    };
                    enemy.Entity.Color = Color.Red;
                    enemy.Entity.Position = spawnPosition + new Vector2(_random.Next(-50, 51), _random.Next(-50, 51));

                    level.Units.Add(enemy);
                }

                level.SpawnTimer = _random.Next(5, 20);

                if (_world.Commanders.Count > 0) {
                    Unit talker = _world.Commanders[_random.Next(_world.Commanders.Count)];

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
                        _world.PlayerMoney += 50;
                    }

                    continue;
                }

                if (unit.Entity.Position.X < level.Position.X
                    || unit.Entity.Position.Y < level.Position.Y
                    || unit.Entity.Position.X > level.Position.X + _levelWidth
                    || unit.Entity.Position.Y > level.Position.Y + _levelHeight) {

                    for (int j = 0; j < _world.Levels.Count; j++) {
                        Level otherLevel = _world.Levels[j];

                        if (otherLevel == level) {
                            continue;
                        }

                        if (unit.Entity.Position.X >= otherLevel.Position.X
                            && unit.Entity.Position.Y >= otherLevel.Position.Y
                            && unit.Entity.Position.X <= otherLevel.Position.X + _levelWidth
                            && unit.Entity.Position.Y <= otherLevel.Position.Y + _levelHeight) {

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
                    unit.Animation.Apply(unit.Entity);
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

        private void DrawEntity(Entity entity) {
            if (entity.Texture is null) {
                return;
            }

            var texture = _content.Load<Texture2D>(entity.Texture);

            _spriteBatch.Draw(
                texture,
                entity.Position + new Vector2(0f, -entity.Depth),
                null,
                entity.Color,
                entity.Rotation,
                texture.Bounds.Size.ToVector2() * entity.Origin,
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

        private void DrawField(Field field) {
            _spriteBatch.Draw(
                _circleTexture,
                field.Entity.Position,
                null,
                Color.Red * 0.5f,
                0f,
                new Vector2(_circleTexture.Width / 2f, _circleTexture.Height / 2f),
                new Vector2(field.Radius * 2f / _circleTexture.Width, field.Radius * 2f / _circleTexture.Height),
                SpriteEffects.None,
                0f);
        }

        private void DrawUnitPath(Unit unit) {
            if (!unit.DrawPath || !unit.TargetPosition.HasValue) {
                return;
            }
            
            _spriteBatch.Draw(
                _pixelTexture,
                unit.Entity.Position,
                null,
                Color.Black,
                GetAngle(unit.TargetPosition.Value - unit.Entity.Position),
                new Vector2(0f, 0.5f),
                new Vector2(Vector2.Distance(unit.Entity.Position, unit.TargetPosition.Value), 1f),
                SpriteEffects.None,
                0f);
        }

        private void DrawUnitOverlay(Unit unit) {
            if (unit.Health >= unit.MaxHealth) {
                return;
            }

            const float healthBarWidth = 40f;
            const float healthBarHeight = 2f;
            
            Vector2 healthBarPosition = unit.Entity.Position + new Vector2(-healthBarWidth / 2f, -70f);

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
                new Vector2(healthBarWidth * unit.PreviousHealth / unit.MaxHealth, healthBarHeight),
                SpriteEffects.None,
                0f);

            _spriteBatch.Draw(
                _pixelTexture,
                healthBarPosition,
                null,
                Color.Red,
                0f,
                Vector2.Zero,
                new Vector2(healthBarWidth * unit.Health / unit.MaxHealth, healthBarHeight),
                SpriteEffects.None,
                0f);
        }

        private bool MouseIntersectsUnit(Unit unit) {
            if (_world.CurrentLevel is null) {
                return false;
            }

            Vector2 worldMousePosition = _world.CurrentLevel.Position + _mouse.Position;

            return Vector2.DistanceSquared(worldMousePosition, unit.Entity.Position - new Vector2(0f, 30f)) < 30f * 30f;
        }

        private void ShowScenario(Scenario scenario) {
            _world.CurrentScenario = scenario;

            Vector2 descriptionSize = _font.MeasureString(scenario.Description.WrapText(_font, 490f));
            Vector2 descriptionPosition = new Vector2(400f - 250f, 300f - descriptionSize.Y / 2f);

            Action? onClick = null;
            if (scenario.Choices.Count == 0 || scenario.Action is not null) {
                onClick = () => {
                    HideScenario();
                    scenario.Action?.Invoke(_world);
                };
            }

            _world.ScenarioElements.Add(new Element {
                Position = descriptionPosition,
                Size = new Vector2(500f, descriptionSize.Y + 10f),
                Label = scenario.Description,
                IsTextBlock = true,
                Margin = 5f,
                OnClick = onClick,
            });

            for (int i = 0; i < scenario.Choices.Count; i++) {
                Choice choice = scenario.Choices[i];

                _world.ScenarioElements.Add(new Element {
                    Position = descriptionPosition + new Vector2(0f, descriptionSize.Y + 10f + 8f + 28f * i),
                    Size = new Vector2(500f, 20f),
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
