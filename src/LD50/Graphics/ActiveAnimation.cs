namespace LD50.Graphics {
    public class ActiveAnimation(Animation animation) {
        private float _timer;

        public bool IsLooping { get; set; }

        public bool IsFinished => !IsLooping && _timer >= animation.GetDuration();

        public void Update(float deltaTime) {
            _timer += deltaTime;
        }

        public string? Apply() {
            float time = _timer;

            if (IsLooping) {
                time %= animation.GetDuration();
            }

            return animation.GetFrame(time);
        }

        public void Reset() {
            _timer = 0f;
        }
    }
}
