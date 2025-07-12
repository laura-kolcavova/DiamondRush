using DiamondRush.MonoGame.Core.Messages;
using DiamondRush.MonoGame.Core.Scenes;
using DiamondRush.MonoGame.Core.Services;
using DiamondRush.MonoGame.Play.Content;
using DiamondRush.MonoGame.Shared.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DiamondRush.MonoGame.Play;

internal sealed class PlayScene : Scene
{
    private readonly GraphicsDevice _graphicsDevice;

    private readonly SpriteBatch _spriteBatch;

    private readonly ContentManager _contentManager;

    private readonly PlaySceneContent _playSceneContent;

    private readonly Messenger _messenger;

    private readonly PlayWorld _playWorld;

    public PlayScene(
        IServiceProvider serviceProvider)
        : base(nameof(PlayScene))
    {
        _graphicsDevice = serviceProvider.GetRequiredService<GraphicsDevice>();

        _spriteBatch = serviceProvider.GetRequiredService<SpriteBatch>();

        _contentManager = new ContentManager(
            serviceProvider,
            AssetNames.RootDirectory);

        _playSceneContent = new PlaySceneContent(
            _contentManager,
            _graphicsDevice);

        _messenger = new Messenger();

        _playWorld = new PlayWorld(
            _graphicsDevice,
            _spriteBatch,
            _messenger,
            _playSceneContent);
    }

    protected override void Initialize()
    {
        base.Initialize();

        _playWorld.Initialize();
    }

    protected override void LoadContent()
    {
        base.LoadContent();

        _playSceneContent.LoadContent();
    }

    protected override void UnloadContent()
    {
        base.UnloadContent();

        _contentManager.Unload();
    }

    protected override void OnDisposing()
    {
        base.OnDisposing();

        _contentManager.Dispose();

        _playSceneContent.Dispose();
    }

    protected override void OnDisposed()
    {
        base.OnDisposed();

        _playWorld.Clear();
    }

    public override void Update(GameTime gameTime)
    {
        _playWorld.Update(gameTime);

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        _graphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch!.Begin();

        _playWorld.Draw(gameTime);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
