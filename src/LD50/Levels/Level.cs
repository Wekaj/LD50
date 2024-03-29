﻿using LD50.Entities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace LD50.Levels {
    public class Level {
        public string Name { get; set; } = "";
        public Vector2 Position { get; set; }
        public List<Unit> Units { get; } = new();
        public List<Projectile> Projectiles { get; } = new();
        public List<Field> Fields { get; } = new();
        public List<Vector2> SpawnPositions { get; } = new();
        public float SpawnTimer { get; set; }
    }
}
