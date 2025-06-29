using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DiamondRush.MonoGame.Core.Textures;

public struct TextureRegion
{
    public required Texture2D Texture { get; set; }

    public required Rectangle SourceBounds { get; set; }
}
