using DiamondRush.MonoGame.Play.Content.Abstractions;
using DiamondRush.MonoGame.Shared.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DiamondRush.MonoGame.Play.Content;

internal sealed class PlaySceneContent :
    IPlaySceneContentProvider,
    IDisposable
{
    private readonly ContentManager _contentManager;

    private readonly GraphicsDevice _graphicsDevice;

    private bool _isDisposed;

    private Texture2D? _blankTexture;

    private Texture2D? _backgroundTexture;

    private Texture2D? _gemSpriteSheet;

    private SpriteFont? _defaultFont;

    public PlaySceneContent(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice)
    {
        _contentManager = contentManager;
        _graphicsDevice = graphicsDevice;
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

    public Texture2D BlankTexture =>
        _blankTexture
        ??= LoadBlankTexture();

    public Texture2D BackgroundTexture =>
        _backgroundTexture
        ??= _contentManager.Load<Texture2D>(AssetNames.Images.Background);

    public Texture2D GemSpriteSheet =>
        _gemSpriteSheet
        ??= _contentManager.Load<Texture2D>(AssetNames.SpriteSheets.Gems);

    public SpriteFont DefaultFont =>
        _defaultFont
        ??= _contentManager.Load<SpriteFont>(AssetNames.Fonts.DefaultFont);

    public void LoadContent()
    {
        _blankTexture = LoadBlankTexture();

        _backgroundTexture = _contentManager.Load<Texture2D>(AssetNames.Images.Background);

        _gemSpriteSheet = _contentManager.Load<Texture2D>(AssetNames.SpriteSheets.Gems);

        _defaultFont = _contentManager.Load<SpriteFont>(AssetNames.Fonts.DefaultFont);
    }

    private Texture2D LoadBlankTexture()
    {
        var texture = new Texture2D(_graphicsDevice, 1, 1);

        texture.SetData([Color.White]);

        return texture;
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

        _blankTexture = null;

        _backgroundTexture = null;

        _gemSpriteSheet = null;

        _defaultFont = null;

        _isDisposed = true;
    }
}
