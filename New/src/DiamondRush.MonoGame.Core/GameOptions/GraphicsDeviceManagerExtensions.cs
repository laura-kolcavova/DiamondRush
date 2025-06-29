using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Core.GameOptions;

public static class GraphicsDeviceManagerExtensions
{
    public static void ApplyGraphicsOptions(
        this GraphicsDeviceManager graphicsDeviceManager,
        GraphicsOptions graphicsOptions)
    {
        graphicsDeviceManager.PreferredBackBufferWidth = graphicsOptions.ResolutionWidth;
        graphicsDeviceManager.PreferredBackBufferHeight = graphicsOptions.ResolutionHeight;
        graphicsDeviceManager.IsFullScreen = graphicsOptions.IsFullScreen;

        graphicsDeviceManager.ApplyChanges();
    }
}
