using LD50.Content;
using LD50.Development;
using LD50.Entities;
using LD50.Graphics;
using LD50.Input;
using LD50.Interface;
using LD50.Levels;
using LD50.Screens;
using LD50.Utilities;
using Microsoft.Xna.Framework;
using SimpleInjector;
using System.Collections.Generic;
using System.Reflection;

namespace LD50 {
    public static class CompositionRoot {
        public static Container CreateContainer(RunArguments runArguments) {
            var container = new Container();

            container.RegisterInstance(runArguments);

            container.RegisterSingleton<Game, LD50Game>();
            container.RegisterSingleton<IGameEvents, LD50Game>();
            container.RegisterSingleton<GraphicsDeviceManager>();
            container.RegisterSingleton<IContentManager, GameContentManagerAdapter>();
            container.RegisterSingleton<IGraphicsDeviceSource, GraphicsDeviceFromManager>();

            container.RegisterSingleton<GameLoop>();

            container.Collection.Register<IStartupHandler>(new[] { Assembly.GetExecutingAssembly() }, Lifestyle.Singleton);
            container.RegisterSingleton<IStartupHandler, GameRunner>();

            container.Collection.Register<IInitializable>(new[] { Assembly.GetExecutingAssembly() }, Lifestyle.Singleton);
            container.RegisterSingleton<IInitializable, CompositeInitializable>();

            container.Collection.Register<IFixedUpdateable>(
                new[] {
                    typeof(XnaMouse),
                    typeof(InputBindings),
                    typeof(WorldUpdater),
                },
                Lifestyle.Singleton);
            container.RegisterSingleton<IFixedUpdateable, CompositeFixedUpdateable>();

            container.Collection.Register<IVariableUpdateable>(new[] { Assembly.GetExecutingAssembly() }, Lifestyle.Singleton);
            container.RegisterSingleton<IVariableUpdateable, CompositeVariableUpdateable>();

            container.Collection.Register<IDrawable>(
                new[] {
                    typeof(WorldDrawer),
                },
                Lifestyle.Singleton);
            container.RegisterSingleton<IDrawable, CompositeDrawable>();

            container.RegisterSingleton<InputBindings>();

            container.RegisterSingleton<AnimationManager>();
            container.RegisterSingleton<XnaMouse>();
            container.RegisterSingleton<InterfaceActions>();

            container.RegisterSingleton<ScreenChanger>();
            container.RegisterSingleton<ScreenManager>();

            var gameScreenProducer = Lifestyle.Singleton.CreateProducer<IScreen, GameScreen>(container);
            var engineScreenProducer = Lifestyle.Singleton.CreateProducer<IScreen, EngineScreen>(container);
            var unitEditorScreenProducer = Lifestyle.Singleton.CreateProducer<IScreen, UnitEditorScreen>(container);
            container.RegisterSingleton<IDictionary<ScreenType, IScreen>>(
                () => new Dictionary<ScreenType, IScreen> {
                    [ScreenType.Game] = gameScreenProducer.GetInstance(),
                    [ScreenType.Engine] = engineScreenProducer.GetInstance(),
                    [ScreenType.UnitEditor] = unitEditorScreenProducer.GetInstance(),
                });

            container.RegisterSingleton<UpdateInfo>();
            container.RegisterSingleton<IGameTimeSource, UpdateInfo>();
            container.RegisterSingleton<IDeltaTimeSource, UpdateInfo>();
            container.RegisterSingleton<SlowUpdateMarker>();
            container.RegisterSingleton<ISlowUpdateMarker, SlowUpdateMarker>();

            container.RegisterSingleton<World>();
            container.RegisterSingleton<ScenarioShower>();
            container.RegisterSingleton<UnitFactory>();
            container.RegisterSingleton<CommanderSelector>();

            container.RegisterSingleton<EngineEnvironment>();
            container.RegisterSingleton<ContentBrowserShower>();

            container.Verify();

            return container;
        }
    }
}
