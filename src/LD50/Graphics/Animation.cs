using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace LD50.Graphics {
    public class Animation {
        private record Frame(Texture2D Texture, float Duration);

        private readonly List<Frame> _frames = new();

        public float Duration { get; private set; }

        public bool IsLooping { get; set; }

        public Animation AddFrame(Texture2D texture, float duration) {
            _frames.Add(new Frame(texture, duration));
            Duration += duration;
            return this;
        }

        public Texture2D? GetFrame(float time) {
            if (_frames.Count == 0) {
                return null;
            }

            if (IsLooping) {
                time %= Duration;
            }

            for (int i = 0; i < _frames.Count - 1; i++) {
                Frame frame = _frames[i];

                if (time < frame.Duration) {
                    return frame.Texture;
                }

                time -= frame.Duration;
            }

            return _frames[^1].Texture;
        }

        public ActiveAnimation Play() {
            return new ActiveAnimation(this);
        }
    }
}
