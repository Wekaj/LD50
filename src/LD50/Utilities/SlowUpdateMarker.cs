namespace LD50.Utilities {
    public class SlowUpdateMarker : ISlowUpdateMarker {
        public bool WasSlowUpdate { get; private set; }
        
        public void MarkSlowUpdate() {
            WasSlowUpdate = true;
        }

        public void Clear() {
            WasSlowUpdate = false;
        }
    }
}
