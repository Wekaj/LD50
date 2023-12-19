using LD50.Content;
using System;
using System.Diagnostics;
using System.IO;

namespace LD50.Utilities {
    public static class CrashLogger {
        public static void Log(Exception exception) {
            string? logDirectory = GetLogDirectory();
            if (logDirectory is null) {
                return;
            }

            string fileName = Path.Combine(logDirectory, $"crash_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");

            var fileInfo = new FileInfo(fileName);
            fileInfo.Directory?.Create();
            
            File.WriteAllText(fileInfo.FullName, exception.ToString());
            
            Process.Start("notepad.exe", fileName);
        }

        private static string? GetLogDirectory() {
            //string? savedGamesDirectory = SaveDataDirectorySource.GetSaveDataDirectory();
            //if (savedGamesDirectory is null) {
            //    return null;
            //}

            //return Path.Combine(savedGamesDirectory, "Logs");
            string assemblyDirectory = FileOpener.GetExecutingDirectory();

            return Path.Combine(assemblyDirectory, "Logs");
        }
    }
}
