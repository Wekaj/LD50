using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace LD50 {
    public class GameRunner(
        Game game,
        IEnumerable<IStartupHandler> startupHandlers)
        : IStartupHandler {

        public void OnStartup() {
            foreach (IStartupHandler handler in startupHandlers) {
                handler.OnStartup();
            }

            game.Run();
        }
    }
}
