using LD50.Development;
using System.IO;
using System.Reflection;

namespace LD50.Content {
    public class FileOpener {
        public static string GetExecutingDirectory() {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
        }

        public static FileStream OpenContentFile(string contentReference) {
            return File.OpenRead(Path.Combine(GetExecutingDirectory(), contentReference));
        }

        public static void CopyContentToExecutingDirectory(EngineEnvironment engineEnvironment, string contentFileName) {
            string relativeName = Path.GetRelativePath(engineEnvironment.ContentDirectory, contentFileName);

            File.Copy(contentFileName, Path.Combine(GetExecutingDirectory(), relativeName), true);
        }
    }
}
