using DiamondRush.MonoGame.Core.Textures;

namespace DiamondRush.MonoGame.Play.Components;

public sealed record GameBoardStyles
{
    public required Sprite BoardSprite { get; init; }

    public required Sprite BorderSprite { get; init; }

    public required Sprite SpacingSprite { get; init; }
}
