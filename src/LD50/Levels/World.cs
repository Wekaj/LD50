using LD50.Entities;
using LD50.Interface;
using LD50.Scenarios;
using LD50.Skills;
using System.Collections.Generic;

namespace LD50.Levels {
    public class World {
        public List<Level> Levels { get; } = [];
        public Level? CurrentLevel { get; set; }

        public int PlayerMoney { get; set; } = 1000;

        public Scenario? CurrentScenario { get; set; }
        public float ScenarioTimer { get; set; }

        public List<Element> Elements { get; } = [];
        public List<Element> ScenarioElements { get; } = [];
        public List<Element> SelectedCommanderElements { get; } = [];

        public List<Popup> Popups { get; } = [];

        public List<Unit> Commanders { get; } = [];
        public Unit? SelectedCommander { get; set; }
        public Skill? CurrentSkill { get; set; }

        public void Reset() {
            Levels.Clear();
            CurrentLevel = null;

            PlayerMoney = 1000;

            CurrentScenario = null;
            ScenarioTimer = 0f;

            Elements.Clear();
            ScenarioElements.Clear();
            SelectedCommanderElements.Clear();

            Commanders.Clear();
            SelectedCommander = null;
            CurrentSkill = null;
        }
    }
}
