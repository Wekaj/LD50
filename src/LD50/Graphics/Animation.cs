using System.Collections.Generic;

namespace LD50.Graphics {
    public class Animation {
        private record Frame(string Texture, float Duration);

        private readonly List<Frame> _frames = new();

        public float Duration { get; private set; }

        public bool IsLooping { get; set; }

        public Animation AddFrame(string texture, float duration) {
            _frames.Add(new Frame(texture, duration));
            Duration += duration;
            return this;
        }

        public string? GetFrame(float time) {
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
