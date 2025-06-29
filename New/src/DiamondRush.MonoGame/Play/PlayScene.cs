using DiamondRush.MonoGame.Core.Scenes;
using DiamondRush.MonoGame.Core.Services;
using DiamondRush.MonoGame.Core.Systems;
using DiamondRush.MonoGame.Play.Content;
using DiamondRush.MonoGame.Play.Factories;
using DiamondRush.MonoGame.Play.Systems;
using DiamondRush.MonoGame.Shared.Assets;
using LightECS;
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

    private readonly EntityContext _entityContext;

    private readonly BackgroundEntityFactory _backgroundEntityFactory;

    private readonly SystemManager _systemManager;

    public PlayScene(
        IServiceProvider serviceProvider)
        : base("PlayScene")
    {
        _graphicsDevice = serviceProvider.GetRequiredService<GraphicsDevice>();
        _spriteBatch = serviceProvider.GetRequiredService<SpriteBatch>();

        _contentManager = new ContentManager(
            serviceProvider,
            AssetNames.RootDirectory);

        _playSceneContent = new PlaySceneContent(
            _contentManager);

        _entityContext = new EntityContext();

        _backgroundEntityFactory = new BackgroundEntityFactory(
            _playSceneContent);

        _systemManager = new SystemManager();
    }

    protected override void Initialize()
    {
        _systemManager.AddSystem(new RenderSystem(
            _entityContext,
            _spriteBatch,
            _graphicsDevice));

        _systemManager.AddSystem(new DiagnosticSystem(
            _spriteBatch,
            _playSceneContent));

        base.Initialize();

        _backgroundEntityFactory.Create(_entityContext);
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

    }

    protected override void OnDisposed()
    {
        base.OnDisposed();

        _systemManager.ClearSystems();
    }

    public override void Update(GameTime gameTime)
    {
        _systemManager.Update(gameTime);

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        _graphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch!.Begin();

        _systemManager.Draw(gameTime);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
