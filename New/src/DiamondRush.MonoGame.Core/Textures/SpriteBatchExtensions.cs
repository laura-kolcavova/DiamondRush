using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DiamondRush.MonoGame.Core.Textures;

public static class SpriteBatchExtensions
{
    public static void DrawSprite(
        this SpriteBatch spriteBatch,
        Sprite sprite,
        Rectangle destinationRectangle)
    {
        spriteBatch.Draw(
            texture: sprite.TextureRegion.Texture,
            destinationRectangle: destinationRectangle,
            sourceRectangle: sprite.TextureRegion.SourceBounds,
            color: sprite.Color,
            rotation: sprite.Rotation,
            origin: sprite.Origin,
            effects: sprite.Effects,
            layerDepth: sprite.LayerDepth);
    }
}
