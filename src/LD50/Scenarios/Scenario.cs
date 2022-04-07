using System.Collections.Generic;

namespace LD50.Scenarios {
    public class Scenario {
        public string Description { get; set; } = "";
        public List<Choice> Choices { get; } = new();
    }
}
