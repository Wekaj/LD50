using System.Collections.Generic;

namespace LD50 {
    public class CompositeDrawable(IEnumerable<IDrawable> drawables)
        : IDrawable {
        
        public void Draw() {
            foreach (IDrawable drawable in drawables) {
                drawable.Draw();
            }
        }
    }
}
