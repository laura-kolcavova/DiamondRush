
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Core;

public sealed record RectTransform
{
    public Vector2 Position { get; init; } = Vector2.Zero;

    public float Width { get; init; } = 0;

    public float Height { get; init; } = 0;

    public float Left => Position.X;

    public float Right => Position.X + Width;

    public float Top => Position.Y;

    public float Bottom => Position.Y + Height;

    public bool Contains(RectTransform rectTransform)
    {
        return Left <= rectTransform.Left
            && Right >= rectTransform.Right
            && Top <= rectTransform.Top
            && Bottom >= rectTransform.Bottom;
    }

    public RectTransform UpdatePosition(
        Vector2 newPosition)
    {
        return this with
        {
            Position = newPosition,
        };
    }
}
