using LD50.Entities;
using System;

namespace LD50.Skills {
    public record Skill {
        public Func<Unit, bool> IsValidTarget { get; init; } = _ => true;
        public Action<Unit> Use { get; init; } = _ => { };
    }
}
