using LD50.Entities;
using System.Collections.Generic;

namespace LD50.Graphics {
    public class AnimationManager {
        public IReadOnlyDictionary<UnitAnimation, Animation> Animations { get; } = new Dictionary<UnitAnimation, Animation> {
            [UnitAnimation.GunnerAttacking] = new Animation()
                .AddFrame("Textures/Gunner_Attack Test 1", 0.1f)
                .AddFrame("Textures/Gunner_Attack2 Test 1", 0.1f)
                .AddFrame("Textures/Gunner_Attack Test 1", 0.1f)
                .AddFrame("Textures/Gunner_Attack2 Test 1", 0.1f)
                .AddFrame("Textures/Gunner_Attack Test 1", 0.1f)
                .AddFrame("Textures/Gunner_Attack2 Test 1", 0.1f),

            [UnitAnimation.BatterAttacking] = new Animation()
                .AddFrame("Textures/Batter_Attack Test 1", 0.075f)
                .AddFrame("Textures/Batter_Attack2 Test 1", 0.075f)
                .AddFrame("Textures/Batter_Attack3 Test 1", 0.3f)
                .AddFrame("Textures/Batter_Attack Test 1", 0.075f),

            [UnitAnimation.RifleWomanAttacking] = new Animation()
                .AddFrame("Textures/RifleWoman_Attack Test 1", 0.1f)
                .AddFrame("Textures/RifleWoman_Attack2 Test 1", 0.2f)
                .AddFrame("Textures/RifleWoman_Attack3 Test 1", 0.2f)
                .AddFrame("Textures/RifleWoman_Attack4 Test 1", 0.2f),

            [UnitAnimation.PistolWomanAttacking] = new Animation()
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
                .AddFrame("Textures/PistolWoman_Attack3 Test 1", 0.1f),

            [UnitAnimation.MinigunLieutenantAttacking] = new Animation()
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
                .AddFrame("Textures/Lieutenant_Attack3 Test 1", 0.075f),

            [UnitAnimation.DaggerLieutenantAttacking] = new Animation()
                .AddFrame("Textures/Lieutenant2_Attack Test 1", 0.075f)
                .AddFrame("Textures/Lieutenant2_Attack2 Test 1", 0.075f)
                .AddFrame("Textures/Lieutenant2_Attack3 Test 1", 0.075f),

            [UnitAnimation.MolotovLieutenantAttacking] = new Animation()
                .AddFrame("Textures/Lieutenant3_Attack Test 1", 0.075f)
                .AddFrame("Textures/Lieutenant3_Attack2 Test 1", 0.075f)
                .AddFrame("Textures/Lieutenant3_Attack3 Test 1", 0.075f)
                .AddFrame("Textures/Lieutenant3_Attack4 Test 1", 0.075f)
                .AddFrame("Textures/Lieutenant3_Attack5 Test 1", 0.075f),
        };
    }
}
