using DiamondRush.MonoGame.Core.Messages.Abstractions;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play.Messages;

internal sealed record PlayerPressReleasedMessage :
    IMessage
{
    public required Vector2 StartPosition { get; init; }

    public required Vector2 EndPosition { get; init; }
}
