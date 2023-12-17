using System.Collections.Generic;

namespace LD50 {
    public class CompositeVariableUpdateable(IEnumerable<IVariableUpdateable> updateables)
        : IVariableUpdateable {
        
        public void VariableUpdate() {
            foreach (IVariableUpdateable updateable in updateables) {
                updateable.VariableUpdate();
            }
        }
    }
}
