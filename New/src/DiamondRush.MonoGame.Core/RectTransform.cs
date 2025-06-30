
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Core;

public sealed record RectTransform
{
    public Vector2 Position { get; init; } = Vector2.Zero;

    public float Width { get; init; } = 0;

    public float Height { get; init; } = 0;
}
