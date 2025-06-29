using DiamondRush.MonoGame.Core.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DiamondRush.MonoGame.Play.Renderers;

internal sealed class BackgroundRenderer
{
    private readonly GraphicsDevice _graphicsDevice;

    public BackgroundRenderer(
        GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
    }

    public void Render(
        SpriteBatch spriteBatch,
        Sprite sprite,
        Vector2 position)
    {
        var width = _graphicsDevice.Viewport.Width;
        var height = _graphicsDevice.Viewport.Height;

        var destinationRectangle = new Rectangle(
            (int)position.X,
            (int)position.Y,
            width,
            height);

        spriteBatch.DrawSprite(
            sprite,
            destinationRectangle);
    }
}
