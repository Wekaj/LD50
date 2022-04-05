using LD50.Input;
using LD50.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SimpleInjector;

namespace LD50 {
    public class LD50Game : Game {
        private readonly GraphicsDeviceManager _graphics;

        private readonly InputBindings _bindings;

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

            _screenManager = container.GetInstance<ScreenManager>();
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                Exit();
            }

            _bindings.Update();
            _screenManager!.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _screenManager!.Draw(gameTime);

            base.Draw(gameTime);
        }

        private Container CreateContainer() {
            var container = new Container();

            container.RegisterInstance(_bindings);
            container.RegisterInstance(Content);
            container.RegisterInstance(new SpriteBatch(GraphicsDevice));

            container.Register<GameScreen>();
            container.Register<ScreenManager>();

            return container;
        }

        private InputBindings CreateBindings() {
            var bindings = new InputBindings();

            bindings.CreateBinding(BindingId.Level1, Keys.D1);
            bindings.CreateBinding(BindingId.Level2, Keys.D2);
            bindings.CreateBinding(BindingId.Level3, Keys.D3);
            bindings.CreateBinding(BindingId.Level4, Keys.D4);

            return bindings;
        }
    }
}
