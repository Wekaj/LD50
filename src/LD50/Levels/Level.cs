using LD50.Entities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace LD50.Levels {
    public class Level {
        public string Name { get; set; } = "";
        public Vector2 Position { get; set; }
        public List<Entity> Entities { get; } = new();
        public List<Vector2> SpawnPositions { get; } = new();
        public float SpawnTimer { get; set; }
    }
}
