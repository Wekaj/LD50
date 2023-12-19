namespace LD50.Screens {
    public enum ScreenType {
        Game,
        Engine,
        UnitEditor,
        AnimationEditor,
        ScenarioEditor,
    }

    public readonly record struct ScreenArgs(
        ScreenType ScreenType,
        bool SkipIntro = false);
}
