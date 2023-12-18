namespace LD50.Screens {
    public enum ScreenType {
        Game,
        Engine,
    }

    public readonly record struct ScreenArgs(ScreenType ScreenType);
}
