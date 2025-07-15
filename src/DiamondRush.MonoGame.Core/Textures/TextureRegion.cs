using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DiamondRush.MonoGame.Core.Textures;

public sealed record TextureRegion
{
    public Texture2D Texture { get; }

    public Rectangle SourceBounds { get; }

    public TextureRegion(Texture2D texture2D)
    {
        Texture = texture2D;
        SourceBounds = new Rectangle(
            0,
            0,
            texture2D.Width,
            texture2D.Height);
    }

    public TextureRegion(
        Texture2D texture2D,
        Rectangle sourceBounds)
    {
        Texture = texture2D;
        SourceBounds = sourceBounds;
    }
}
