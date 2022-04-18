using LD50.Entities;
using System;

namespace LD50.Skills {
    public record Skill {
        public Func<Entity, bool> IsValidTarget { get; init; } = _ => true;
        public Action<Entity> Use { get; init; } = _ => { };
    }
}
