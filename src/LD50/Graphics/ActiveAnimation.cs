using LD50.Entities;

namespace LD50.Graphics {
    public class ActiveAnimation {
        private readonly Animation _animation;

        private float _timer;

        public ActiveAnimation(Animation animation) {
            _animation = animation;
        }

        public bool IsFinished => !_animation.IsLooping && _timer >= _animation.Duration;

        public void Update(float deltaTime) {
            _timer += deltaTime;
        }

        public void Apply(Entity entity) {
            entity.Texture = _animation.GetFrame(_timer) ?? entity.Texture;
        }
    }
}
