using Microsoft.Xna.Framework.Graphics;

namespace DiamondRush.MonoGame.Play.Content.Abstractions;

internal interface IPlaySceneContentProvider
{
    public Texture2D BackgroundTexture { get; }

    public SpriteFont DefaultFont { get; }

    public Texture2D BlankTexture { get; }
}
