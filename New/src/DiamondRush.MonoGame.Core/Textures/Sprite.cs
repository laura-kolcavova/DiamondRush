using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DiamondRush.MonoGame.Core.Textures;

public struct Sprite
{
    public required TextureRegion TextureRegion { get; set; }

    public Color Color { get; set; } = Color.White;

    public float Rotation { get; set; } = 0.0f;

    public Vector2 Scale { get; set; } = Vector2.One;

    public Vector2 Origin { get; set; } = Vector2.Zero;

    public SpriteEffects Effects { get; set; } = SpriteEffects.None;

    public float LayerDepth { get; set; } = 0.0f;

    public readonly float Width => TextureRegion.SourceBounds.Width * Scale.X;

    public readonly float Height => TextureRegion.SourceBounds.Width * Scale.X;

    public Sprite()
    {
    }
}
