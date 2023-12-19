using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LD50.Graphics {
    public record Frame {
        public string? Texture { get; set; }
        public float Duration { get; set; }
    }

    public record Animation {
        public List<Frame> Frames { get; set; } = [];

        public string? GetFrame(float time) {
            if (Frames.Count == 0) {
                return null;
            }

            for (int i = 0; i < Frames.Count - 1; i++) {
                Frame frame = Frames[i];

                if (time < frame.Duration) {
                    return frame.Texture;
                }

                time -= frame.Duration;
            }

            return Frames[^1].Texture;
        }

        public ActiveAnimation Play() {
            return new ActiveAnimation(this);
        }

        public float GetDuration() {
            float duration = 0f;
            for (int i = 0; i < Frames.Count; i++) {
                duration += Frames[i].Duration;
            }
            return duration;
        }
    }
}
