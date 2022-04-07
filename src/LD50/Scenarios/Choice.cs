using LD50.Levels;
using System;

namespace LD50.Scenarios {
    public class Choice {
        public string Label { get; set; } = "";
        public Action<World> Action { get; set; } = _ => { };
    }
}
