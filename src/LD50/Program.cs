using LD50;
using LD50.Utilities;
using SimpleInjector;
using System;
using System.Diagnostics;
using System.Threading;

try {
    Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
    Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

    var runArguments = RunArguments.FromArray(args);
    using Container container = CompositionRoot.CreateContainer(runArguments);

    var startupHandler = container.GetInstance<IStartupHandler>();
    startupHandler.OnStartup();
}
catch (Exception exception) when (!Debugger.IsAttached) {
    CrashLogger.Log(exception);
}
