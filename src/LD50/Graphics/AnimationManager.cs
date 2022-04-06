using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LD50.Graphics {
    public class AnimationManager {
        public AnimationManager(ContentManager content) {
            BatterAttacking = new Animation()
                .AddFrame(content.Load<Texture2D>("Textures/Batter_Attack Test 1"), 0.075f)
                .AddFrame(content.Load<Texture2D>("Textures/Batter_Attack2 Test 1"), 0.075f)
                .AddFrame(content.Load<Texture2D>("Textures/Batter_Attack3 Test 1"), 0.3f)
                .AddFrame(content.Load<Texture2D>("Textures/Batter_Attack Test 1"), 0.075f);
        }

        public Animation BatterAttacking { get; }
    }
}
