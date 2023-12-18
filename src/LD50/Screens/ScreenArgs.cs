namespace LD50.Screens {
    public enum ScreenType {
        Game,
        Engine,
        UnitEditor,
    }

    public readonly record struct ScreenArgs(ScreenType ScreenType);
}
