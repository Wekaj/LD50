using LD50.Levels;
using System;
using System.Collections.Generic;

namespace LD50.Scenarios {
    public class Scenario {
        public string Description { get; set; } = "";
        public List<Choice> Choices { get; set; } = new();
        public Action<World>? Action { get; set; }
    }
}
