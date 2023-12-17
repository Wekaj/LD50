using LD50.Content;
using Microsoft.Xna.Framework;

namespace LD50 {
    public class GameContentManagerAdapter : IContentManager {
        private readonly Game _game;

        public GameContentManagerAdapter(Game game) {
            _game = game;

            //_game.Content.RootDirectory = "res";
        }

        public string RootDirectory => _game.Content.RootDirectory;

        public T Load<T>(string path) {
            return _game.Content.Load<T>(path);
        }
    }
}
