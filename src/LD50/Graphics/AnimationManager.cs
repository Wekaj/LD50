using LD50.Content;
using System.IO;
using System.Text.Json;

namespace LD50.Graphics {
    public class AnimationManager {
        public Animation GetAnimation(string animationReference) {
            using FileStream stream = FileOpener.OpenContentFile(animationReference);
            
            return JsonSerializer.Deserialize<Animation>(stream)!;
        }
    }
}
