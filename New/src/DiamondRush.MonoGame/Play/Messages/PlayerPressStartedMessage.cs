using DiamondRush.MonoGame.Core.Messages.Abstractions;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play.Messages;

internal sealed record PlayerPressStartedMessage :
    IMessage
{
    public required Vector2 Position { get; init; }
}
