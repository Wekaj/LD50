using System;

namespace LD50 {
    public interface IGameEvents {
        event EventHandler? Drawn;
        event EventHandler? Initialized;
        event EventHandler? Updated;
    }
}