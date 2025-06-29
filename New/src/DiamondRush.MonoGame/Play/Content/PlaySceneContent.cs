using DiamondRush.MonoGame.Play.Content.Abstractions;
using DiamondRush.MonoGame.Shared.Assets;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DiamondRush.MonoGame.Play.Content;

internal sealed class PlaySceneContent :
    IPlaySceneContentProvider,
    IDisposable
{
    private readonly ContentManager _contentManager;

    private bool _isDisposed;

    private Texture2D? _backgroundTexture;

    private SpriteFont? _defaultFont;

    public PlaySceneContent(ContentManager contentManager)
    {
        _contentManager = contentManager;
    }

    ~PlaySceneContent()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public Texture2D BackgroundTexture =>
        _backgroundTexture
        ??= _contentManager.Load<Texture2D>(AssetNames.Textures.Background);

    public SpriteFont DefaultFont =>
        _defaultFont
        ??= _contentManager.Load<SpriteFont>(AssetNames.Fonts.DefaultFont);

    public void LoadContent()
    {
        _backgroundTexture = _contentManager.Load<Texture2D>(AssetNames.Textures.Background);

        _defaultFont = _contentManager.Load<SpriteFont>(AssetNames.Fonts.DefaultFont);
    }

    private void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (disposing)
        {
        }

        _backgroundTexture = null;

        _isDisposed = true;
    }
}
