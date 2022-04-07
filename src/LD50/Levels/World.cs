using LD50.Scenarios;
using System.Collections.Generic;

namespace LD50.Levels {
    public class World {
        public List<Level> Levels { get; } = new();
        public Level? CurrentLevel { get; set; }

        public int PlayerMoney { get; set; }

        public Scenario? CurrentScenario { get; set; }
        public float ScenarioTimer { get; set; }
    }
}
