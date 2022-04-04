using LD50.Entities;
using System.Collections.Generic;

namespace LD50.Levels {
    public class Level {
        public string Name { get; set; } = "";
        public List<Entity> Entities { get; } = new();
    }
}
