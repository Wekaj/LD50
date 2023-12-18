using System.IO;

namespace LD50.Development {
    public class EngineEnvironment(RunArguments runArguments)
        : IStartupHandler {

        public string? ProjectDirectory { get; private set; }

        public void OnStartup() {
            if (runArguments.ProjectDirectory is null) {
                return;
            }

            ProjectDirectory = Path.GetFullPath(runArguments.ProjectDirectory);

            if (!Directory.Exists(ProjectDirectory)) {
                throw new DirectoryNotFoundException($"Project directory not found: {ProjectDirectory}");
            }
        }
    }
}
