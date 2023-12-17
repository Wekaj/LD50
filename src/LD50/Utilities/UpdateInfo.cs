using Microsoft.Xna.Framework;
using System;

namespace LD50.Utilities {
    public class UpdateInfo : IGameTimeSource, IDeltaTimeSource, IBlendFactorSource {
        private readonly GameTime _latestGameTime = new();
        private float _latestDeltaTime;
        private float _latestBlendFactor;

        GameTime IGameTimeSource.Latest => _latestGameTime;
        float IDeltaTimeSource.Latest => _latestDeltaTime;
        float IBlendFactorSource.Latest => _latestBlendFactor;

        public void UpdateTotalTime(TimeSpan totalTime) {
            _latestGameTime.TotalGameTime = totalTime;
        }

        public void UpdateDeltaTime(TimeSpan deltaTime) {
            _latestGameTime.ElapsedGameTime = deltaTime;
            _latestDeltaTime = (float)deltaTime.TotalSeconds;
        }

        public void UpdateBlendFactor(float blendFactor) {
            _latestBlendFactor = blendFactor;
        }
    }
}
