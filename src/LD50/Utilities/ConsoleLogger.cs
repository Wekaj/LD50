using System;

namespace LD50.Utilities {
    public class ConsoleLogger : ILogger {
        public void Write(string text, ConsoleMessageType type) {
            switch (type) {
                case ConsoleMessageType.Command:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case ConsoleMessageType.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case ConsoleMessageType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case ConsoleMessageType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
