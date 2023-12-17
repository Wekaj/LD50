using System;

namespace LD50.Utilities {
    public enum ConsoleMessageType {
        Command,
        Info,
        Warning,
        Error,
    }

    public interface ILogger {
        void Write(string text, ConsoleMessageType type);
    }

    public static class ILoggerExtensions {
        public static void WriteInfo(this ILogger logger, string text) {
            logger.Write(text, ConsoleMessageType.Info);
        }

        public static void WriteWarning(this ILogger logger, string text) {
            logger.Write(text, ConsoleMessageType.Warning);
        }

        public static void WriteError(this ILogger logger, string text) {
            logger.Write(text, ConsoleMessageType.Error);
        }
    }
}
