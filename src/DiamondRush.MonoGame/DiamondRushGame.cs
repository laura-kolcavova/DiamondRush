using DiamondRush.MonoGame.Core.GameOptions;
using DiamondRush.MonoGame.Core.Scenes;
using DiamondRush.MonoGame.Play;
using DiamondRush.MonoGame.Shared.Assets;
using DiamondRush.MonoGame.Shared.GameOptions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DiamondRush.MonoGame;

internal sealed class DiamondRushGame : Game
{
    public const string Title = "Diamond Rush";

    private readonly GraphicsDeviceManager _graphics;

    private readonly SceneManager _sceneManager;

    private SpriteBatch? _spriteBatch;

    public DiamondRushGame()
    {
        var graphicsOptions = DefaultGraphicsOptionsProvider.Get();

        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferFormat = SurfaceFormat.Color,
            PreferMultiSampling = false,
            PreferredDepthStencilFormat = DepthFormat.None,
            SynchronizeWithVerticalRetrace = true,
        };

        _graphics.ApplyGraphicsOptions(graphicsOptions);

        _sceneManager = new SceneManager();

        Window.Title = Title;
        Window.AllowUserResizing = false;

        Content.RootDirectory = AssetNames.RootDirectory;

        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromTicks(166667L); // ~60 FPS

        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Services.AddService(_spriteBatch);
        Services.AddService(_graphics);
        Services.AddService(GraphicsDevice);
        Services.AddService(_sceneManager);

        base.Initialize();

        var playScene = new PlayScene(Services);

        playScene.Load();

        _sceneManager.SetActiveScene(playScene);
    }

    protected override void LoadContent()
    {
    }

    protected override void Update(GameTime gameTime)
    {
        _sceneManager
            .GetActiveScene()
            .Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _sceneManager
            .GetActiveScene()
            .Draw(gameTime);

        base.Draw(gameTime);
    }
}
