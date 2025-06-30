using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DiamondRush.MonoGame.Core.Textures;

public sealed record Sprite
{
    public required TextureRegion TextureRegion { get; init; }

    public Color Color { get; init; } = Color.White;

    public float Rotation { get; init; } = 0.0f;

    public Vector2 Scale { get; init; } = Vector2.One;

    public Vector2 Origin { get; init; } = Vector2.Zero;

    public SpriteEffects Effects { get; init; } = SpriteEffects.None;

    public float LayerDepth { get; init; } = 0.0f;

    public float Width => TextureRegion.SourceBounds.Width * Scale.X;

    public float Height => TextureRegion.SourceBounds.Width * Scale.X;
}
