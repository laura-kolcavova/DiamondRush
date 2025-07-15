using DiamondRush.MonoGame.Core.GameOptions;

namespace DiamondRush.MonoGame.Shared.GameOptions;

internal static class DefaultGraphicsOptionsProvider
{
    public static GraphicsOptions Get() =>
        new()
        {
            ResolutionWidth = 1440,
            ResolutionHeight = 900,
            IsFullScreen = false,
        };
}
