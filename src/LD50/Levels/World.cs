using LD50.Entities;
using LD50.Interface;
using LD50.Scenarios;
using System.Collections.Generic;

namespace LD50.Levels {
    public class World {
        public List<Level> Levels { get; } = new();
        public Level? CurrentLevel { get; set; }

        public int PlayerMoney { get; set; } = 1000;

        public Scenario? CurrentScenario { get; set; }
        public float ScenarioTimer { get; set; }
        public List<Element> ScenarioElements { get; } = new();

        public List<Element> Elements { get; } = new();

        public List<Entity> Commanders { get; } = new();
        public Entity? SelectedCommander { get; set; }
    }
    
}
