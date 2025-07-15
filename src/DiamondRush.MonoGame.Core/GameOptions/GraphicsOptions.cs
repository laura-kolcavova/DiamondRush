namespace DiamondRush.MonoGame.Core.GameOptions;

public sealed record GraphicsOptions
{
    public required int ResolutionWidth { get; init; }

    public required int ResolutionHeight { get; init; }

    public required bool IsFullScreen { get; init; }
}
