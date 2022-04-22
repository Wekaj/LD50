namespace LD50.Entities {
    public class Field {
        public Entity Entity { get; } = new();

        public Unit? Source { get; set; }

        public float Life { get; set; }
        public float Radius { get; set; }
        public int DamagePerTick { get; set; }
        public float TickInterval { get; set; }
        public float TickTimer { get; set; }
    }
}
