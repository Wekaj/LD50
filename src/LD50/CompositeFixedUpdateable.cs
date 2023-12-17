using System.Collections.Generic;

namespace LD50 {
    public class CompositeFixedUpdateable(IEnumerable<IFixedUpdateable> updateables)
        : IFixedUpdateable {

        public void FixedUpdate() {
            foreach (IFixedUpdateable updateable in updateables) {
                updateable.FixedUpdate();
            }
        }
    }
}
