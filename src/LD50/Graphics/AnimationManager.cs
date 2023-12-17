namespace LD50.Graphics {
    public class AnimationManager : IStartupHandler {
        public void OnStartup() {
            GunnerAttacking = new Animation()
                .AddFrame("Textures/Gunner_Attack Test 1", 0.1f)
                .AddFrame("Textures/Gunner_Attack2 Test 1", 0.1f)
                .AddFrame("Textures/Gunner_Attack Test 1", 0.1f)
                .AddFrame("Textures/Gunner_Attack2 Test 1", 0.1f)
                .AddFrame("Textures/Gunner_Attack Test 1", 0.1f)
                .AddFrame("Textures/Gunner_Attack2 Test 1", 0.1f);

            BatterAttacking = new Animation()
                .AddFrame("Textures/Batter_Attack Test 1", 0.075f)
                .AddFrame("Textures/Batter_Attack2 Test 1", 0.075f)
                .AddFrame("Textures/Batter_Attack3 Test 1", 0.3f)
                .AddFrame("Textures/Batter_Attack Test 1", 0.075f);

            RifleWomanAttacking = new Animation()
                .AddFrame("Textures/RifleWoman_Attack Test 1", 0.1f)
                .AddFrame("Textures/RifleWoman_Attack2 Test 1", 0.2f)
                .AddFrame("Textures/RifleWoman_Attack3 Test 1", 0.2f)
                .AddFrame("Textures/RifleWoman_Attack4 Test 1", 0.2f);

            PistolWomanAttacking = new Animation()
                .AddFrame("Textures/PistolWoman_Attack Test 1", 0.1f)
                .AddFrame("Textures/PistolWoman_Attack2 Test 1", 0.1f)
                .AddFrame("Textures/PistolWoman_Attack3 Test 1", 0.1f)
                .AddFrame("Textures/PistolWoman_Attack2 Test 1", 0.1f)
                .AddFrame("Textures/PistolWoman_Attack3 Test 1", 0.1f)
                .AddFrame("Textures/PistolWoman_Attack2 Test 1", 0.1f)
                .AddFrame("Textures/PistolWoman_Attack3 Test 1", 0.1f)
                .AddFrame("Textures/PistolWoman_Attack2 Test 1", 0.1f)
                .AddFrame("Textures/PistolWoman_Attack3 Test 1", 0.1f)
                .AddFrame("Textures/PistolWoman_Attack2 Test 1", 0.1f)
                .AddFrame("Textures/PistolWoman_Attack3 Test 1", 0.1f);

            MinigunLieutenantAttacking = new Animation()
                .AddFrame("Textures/Lieutenant_Attack Test 1", 0.075f)
                .AddFrame("Textures/Lieutenant_Attack2 Test 1", 0.075f)
                .AddFrame("Textures/Lieutenant_Attack3 Test 1", 0.075f)
                .AddFrame("Textures/Lieutenant_Attack2 Test 1", 0.075f)
                .AddFrame("Textures/Lieutenant_Attack3 Test 1", 0.075f)
                .AddFrame("Textures/Lieutenant_Attack2 Test 1", 0.075f)
                .AddFrame("Textures/Lieutenant_Attack3 Test 1", 0.075f)
                .AddFrame("Textures/Lieutenant_Attack2 Test 1", 0.075f)
                .AddFrame("Textures/Lieutenant_Attack3 Test 1", 0.075f)
                .AddFrame("Textures/Lieutenant_Attack2 Test 1", 0.075f)
                .AddFrame("Textures/Lieutenant_Attack3 Test 1", 0.075f);

            DaggerLieutenantAttacking = new Animation()
                .AddFrame("Textures/Lieutenant2_Attack Test 1", 0.075f)
                .AddFrame("Textures/Lieutenant2_Attack2 Test 1", 0.075f)
                .AddFrame("Textures/Lieutenant2_Attack3 Test 1", 0.075f);

            MolotovLieutenantAttacking = new Animation()
                .AddFrame("Textures/Lieutenant3_Attack Test 1", 0.075f)
                .AddFrame("Textures/Lieutenant3_Attack2 Test 1", 0.075f)
                .AddFrame("Textures/Lieutenant3_Attack3 Test 1", 0.075f)
                .AddFrame("Textures/Lieutenant3_Attack4 Test 1", 0.075f)
                .AddFrame("Textures/Lieutenant3_Attack5 Test 1", 0.075f);
        }

        public Animation GunnerAttacking { get; private set; } = new();
        public Animation BatterAttacking { get; private set; } = new();
        public Animation RifleWomanAttacking { get; private set; } = new();
        public Animation PistolWomanAttacking { get; private set; } = new();
        public Animation MinigunLieutenantAttacking { get; private set; } = new();
        public Animation DaggerLieutenantAttacking { get; private set; } = new();
        public Animation MolotovLieutenantAttacking { get; private set; } = new();
    }
}
