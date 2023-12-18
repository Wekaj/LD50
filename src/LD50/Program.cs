using LD50;
using SimpleInjector;

var runArguments = RunArguments.FromArray(args);
using Container container = CompositionRoot.CreateContainer(runArguments);

var startupHandler = container.GetInstance<IStartupHandler>();
startupHandler.OnStartup();
