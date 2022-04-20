using LD50.Graphics;
using LD50.Input;
using LD50.Interface;
using LD50.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SimpleInjector;

namespace LD50 {
    public class LD50Game : Game {
        private readonly GraphicsDeviceManager _graphics;

        private readonly InputBindings _bindings;

        private XnaMouse? _mouse;
        private ScreenManager? _screenManager;

        public LD50Game() {
            _graphics = new GraphicsDeviceManager(this);

            _bindings = CreateBindings();

            IsMouseVisible = true;
        }

        protected override void Initialize() {
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent() {
            Container container = CreateContainer();

            _mouse = container.GetInstance<XnaMouse>();
            _screenManager = container.GetInstance<ScreenManager>();
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                Exit();
            }

            _bindings.Update();
            _mouse?.Update();
            _screenManager!.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.DimGray);

            _screenManager!.Draw(gameTime);

            base.Draw(gameTime);
        }

        private Container CreateContainer() {
            var container = new Container();

            container.RegisterInstance(_bindings);
            container.RegisterInstance(Content);
            container.RegisterInstance(Window);
            container.RegisterInstance(new SpriteBatch(GraphicsDevice));

            container.RegisterSingleton<AnimationManager>();
            container.RegisterSingleton<XnaMouse>();
            container.RegisterSingleton<InterfaceActions>();

            container.Register<GameScreen>();
            container.Register<ScreenManager>();

            return container;
        }

        private InputBindings CreateBindings() {
            var bindings = new InputBindings();

            bindings.CreateBinding(BindingId.Select, MouseButton.Left);
            bindings.CreateBinding(BindingId.Move, MouseButton.Right);
            
            bindings.CreateBinding(BindingId.Commander1, Keys.D1);
            bindings.CreateBinding(BindingId.Commander2, Keys.D2);
            bindings.CreateBinding(BindingId.Commander3, Keys.D3);
            
            bindings.CreateBinding(BindingId.Level1, Keys.Q);
            bindings.CreateBinding(BindingId.Level2, Keys.W);
            bindings.CreateBinding(BindingId.Level3, Keys.A);
            bindings.CreateBinding(BindingId.Level4, Keys.S);

            bindings.CreateBinding(BindingId.Action1, Keys.Z);
            bindings.CreateBinding(BindingId.Action2, Keys.X);
            bindings.CreateBinding(BindingId.Action3, Keys.C);

            return bindings;
        }
    }
}
