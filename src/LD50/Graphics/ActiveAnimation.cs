namespace LD50.Graphics {
    public class ActiveAnimation(Animation animation) {
        private float _timer;

        public bool IsLooping { get; set; }

        public bool IsFinished => !IsLooping && _timer >= animation.Duration;

        public void Update(float deltaTime) {
            _timer += deltaTime;
        }

        public string? Apply() {
            float time = _timer;

            if (IsLooping) {
                time %= animation.Duration;
            }

            return animation.GetFrame(time);
        }
    }
}
