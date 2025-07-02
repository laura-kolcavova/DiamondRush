using Microsoft.Xna.Framework.Graphics;

namespace DiamondRush.MonoGame.Play.Content.Abstractions;

internal interface IPlaySceneContentProvider
{
    public Texture2D BlankTexture { get; }

    public Texture2D BackgroundTexture { get; }

    public Texture2D GemsTexture { get; }

    public SpriteFont DefaultFont { get; }
}
