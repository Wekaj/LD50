using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Text;

namespace LD50.Utilities {
    public static class StringExtensions {
        public static string WrapText(this string text, SpriteFont spriteFont, float maxLineWidth) {
            string[] words = text.Split(' ');
            var sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = spriteFont.MeasureString(" ").X;

            foreach (string word in words) {
                Vector2 size = spriteFont.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth) {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
        }

        public static string GetContentName(this string path) {
            path = path.Replace('/', '\\');
            
            path = path[(path.IndexOf('\\') + 1)..];

            return Path.GetFileNameWithoutExtension(path);
        }
    }
}
