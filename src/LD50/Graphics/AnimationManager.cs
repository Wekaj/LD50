using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LD50.Graphics {
    public class AnimationManager {
        public AnimationManager(ContentManager content) {
            GunnerAttacking = new Animation()
                .AddFrame(content.Load<Texture2D>("Textures/Gunner_Attack Test 1"), 0.1f)
                .AddFrame(content.Load<Texture2D>("Textures/Gunner_Attack2 Test 1"), 0.1f)
                .AddFrame(content.Load<Texture2D>("Textures/Gunner_Attack Test 1"), 0.1f)
                .AddFrame(content.Load<Texture2D>("Textures/Gunner_Attack2 Test 1"), 0.1f)
                .AddFrame(content.Load<Texture2D>("Textures/Gunner_Attack Test 1"), 0.1f)
                .AddFrame(content.Load<Texture2D>("Textures/Gunner_Attack2 Test 1"), 0.1f);

            BatterAttacking = new Animation()
                .AddFrame(content.Load<Texture2D>("Textures/Batter_Attack Test 1"), 0.075f)
                .AddFrame(content.Load<Texture2D>("Textures/Batter_Attack2 Test 1"), 0.075f)
                .AddFrame(content.Load<Texture2D>("Textures/Batter_Attack3 Test 1"), 0.3f)
                .AddFrame(content.Load<Texture2D>("Textures/Batter_Attack Test 1"), 0.075f);

            RifleWomanAttacking = new Animation()
                .AddFrame(content.Load<Texture2D>("Textures/RifleWoman_Attack Test 1"), 0.1f)
                .AddFrame(content.Load<Texture2D>("Textures/RifleWoman_Attack2 Test 1"), 0.2f)
                .AddFrame(content.Load<Texture2D>("Textures/RifleWoman_Attack3 Test 1"), 0.2f)
                .AddFrame(content.Load<Texture2D>("Textures/RifleWoman_Attack4 Test 1"), 0.2f);

            PistolWomanAttacking = new Animation()
                .AddFrame(content.Load<Texture2D>("Textures/PistolWoman_Attack Test 1"), 0.1f)
                .AddFrame(content.Load<Texture2D>("Textures/PistolWoman_Attack2 Test 1"), 0.1f)
                .AddFrame(content.Load<Texture2D>("Textures/PistolWoman_Attack3 Test 1"), 0.1f)
                .AddFrame(content.Load<Texture2D>("Textures/PistolWoman_Attack2 Test 1"), 0.1f)
                .AddFrame(content.Load<Texture2D>("Textures/PistolWoman_Attack3 Test 1"), 0.1f)
                .AddFrame(content.Load<Texture2D>("Textures/PistolWoman_Attack2 Test 1"), 0.1f)
                .AddFrame(content.Load<Texture2D>("Textures/PistolWoman_Attack3 Test 1"), 0.1f)
                .AddFrame(content.Load<Texture2D>("Textures/PistolWoman_Attack2 Test 1"), 0.1f)
                .AddFrame(content.Load<Texture2D>("Textures/PistolWoman_Attack3 Test 1"), 0.1f)
                .AddFrame(content.Load<Texture2D>("Textures/PistolWoman_Attack2 Test 1"), 0.1f)
                .AddFrame(content.Load<Texture2D>("Textures/PistolWoman_Attack3 Test 1"), 0.1f);

            MinigunLieutenantAttacking = new Animation()
                .AddFrame(content.Load<Texture2D>("Textures/Lieutenant_Attack Test 1"), 0.075f)
                .AddFrame(content.Load<Texture2D>("Textures/Lieutenant_Attack2 Test 1"), 0.075f)
                .AddFrame(content.Load<Texture2D>("Textures/Lieutenant_Attack3 Test 1"), 0.075f)
                .AddFrame(content.Load<Texture2D>("Textures/Lieutenant_Attack2 Test 1"), 0.075f)
                .AddFrame(content.Load<Texture2D>("Textures/Lieutenant_Attack3 Test 1"), 0.075f)
                .AddFrame(content.Load<Texture2D>("Textures/Lieutenant_Attack2 Test 1"), 0.075f)
                .AddFrame(content.Load<Texture2D>("Textures/Lieutenant_Attack3 Test 1"), 0.075f)
                .AddFrame(content.Load<Texture2D>("Textures/Lieutenant_Attack2 Test 1"), 0.075f)
                .AddFrame(content.Load<Texture2D>("Textures/Lieutenant_Attack3 Test 1"), 0.075f)
                .AddFrame(content.Load<Texture2D>("Textures/Lieutenant_Attack2 Test 1"), 0.075f)
                .AddFrame(content.Load<Texture2D>("Textures/Lieutenant_Attack3 Test 1"), 0.075f);

            DaggerLieutenantAttacking = new Animation()
                .AddFrame(content.Load<Texture2D>("Textures/Lieutenant2_Attack Test 1"), 0.075f)
                .AddFrame(content.Load<Texture2D>("Textures/Lieutenant2_Attack2 Test 1"), 0.075f)
                .AddFrame(content.Load<Texture2D>("Textures/Lieutenant2_Attack3 Test 1"), 0.075f);
        }

        public Animation GunnerAttacking { get; }
        public Animation BatterAttacking { get; }
        public Animation RifleWomanAttacking { get; }
        public Animation PistolWomanAttacking { get; }
        public Animation MinigunLieutenantAttacking { get; }
        public Animation DaggerLieutenantAttacking { get; }
    }
}
