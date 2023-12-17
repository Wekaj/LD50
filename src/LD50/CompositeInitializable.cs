using System.Collections.Generic;

namespace LD50 {
    public class CompositeInitializable(IEnumerable<IInitializable> initializables)
        : IInitializable {

        public void Initialize() {
            foreach (IInitializable initializable in initializables) {
                initializable.Initialize();
            }
        }
    }
}
