using DiamondRush.MonoGame.Core.Messages.Abstractions;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play.Messages;

internal sealed record PlayerPressMovedMessage :
    IMessage
{
    public required Vector2 StartPosition { get; init; }

    public required Vector2 CurrentPosition { get; init; }
}
