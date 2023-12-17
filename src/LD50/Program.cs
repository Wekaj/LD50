using LD50;
using SimpleInjector;

using Container container = CompositionRoot.CreateContainer();

var startupHandler = container.GetInstance<IStartupHandler>();
startupHandler.OnStartup();
