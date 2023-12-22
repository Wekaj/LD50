namespace LD50 {
    public record RunArguments {
        /// <summary>
        /// The project directory. Used for development.
        /// </summary>
        public string? ProjectDirectory { get; init; }

        public static RunArguments FromArray(string[] args) {
            return new RunArguments {
                ProjectDirectory = GetArgument(args, "project"),
            };
        }

        private static bool HasArgument(string[] args, string name) {
            for (int i = 0; i < args.Length; i++) {
                if (args[i] == $"-{name}") {
                    return true;
                }
            }

            return false;
        }

        private static string? GetArgument(string[] args, string name) {
            for (int i = 0; i < args.Length - 1; i++) {
                if (args[i] == $"-{name}") {
                    return args[i + 1];
                }
            }

            return null;
        }
    }
}
