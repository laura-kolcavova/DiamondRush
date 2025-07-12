using DiamondRush.MonoGame.Core;
using DiamondRush.MonoGame.Core.Messages.Abstractions;
using DiamondRush.MonoGame.Core.Systems;
using DiamondRush.MonoGame.Play.Components;
using DiamondRush.MonoGame.Play.Content.Abstractions;
using DiamondRush.MonoGame.Play.Factories;
using DiamondRush.MonoGame.Play.Systems;
using LightECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DiamondRush.MonoGame.Play;

internal sealed class PlayWorld
{
    private readonly GraphicsDevice _graphicsDevice;

    private readonly SpriteBatch _spriteBatch;

    private readonly IMessenger _messenger;

    private readonly IPlaySceneContentProvider _playSceneContentProvider;

    private readonly EntityContext _entityContext;

    private readonly SystemManager _systemManager;

    public PlayWorld(
        GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch,
        IMessenger messenger,
        IPlaySceneContentProvider playSceneContentProvider)
    {
        _graphicsDevice = graphicsDevice;

        _spriteBatch = spriteBatch;

        _messenger = messenger;

        _playSceneContentProvider = playSceneContentProvider;

        _entityContext = new EntityContext();

        _systemManager = new SystemManager();
    }

    public void Initialize()
    {
        var backgroundEntityFactory = new BackgroundEntityFactory(
           _playSceneContentProvider,
           _graphicsDevice);

        var gameBoardEntityFactory = new GameBoardEntityFactory(
            _playSceneContentProvider,
            _graphicsDevice);

        backgroundEntityFactory.Create(_entityContext);

        var gameBoardEntity = gameBoardEntityFactory.Create(
            _entityContext,
            Constants.GameBoardRows,
            Constants.GameBoardColumns);

        var gameBoard = _entityContext.Get<GameBoard>(gameBoardEntity);

        var gameBoardRectTransform = _entityContext.Get<RectTransform>(gameBoardEntity);

        var playContext = PlayContext.CreateDefault(
            gameBoardEntity,
            gameBoard);

        playContext.ComputeGameBoardFieldPositions(gameBoardRectTransform);

        playContext.SetPlayState(PlayState.SpawningNewGems);

        _systemManager.AddSystem(
            new GemSpawnSystem(
                _entityContext,
                _playSceneContentProvider,
                playContext));

        _systemManager.AddSystem(
            new GemFallSystem(
                _entityContext,
                playContext));

        _systemManager.AddSystem(
            new GemMatchSystem(
                _entityContext,
                playContext));

        _systemManager.AddSystem(
            new GemCollectSystem(
                _entityContext,
                _messenger,
                playContext));

        _systemManager.AddSystem(
            new GemDragSystem(
                _entityContext,
                _messenger,
                playContext));

        _systemManager.AddSystem(
            new GemAnimationSystem(
                _entityContext));

        _systemManager.AddSystem(
            new GemDestroySystem(
                _entityContext));

        _systemManager.AddSystem(
            new SoundEffectSystem(
                _messenger,
                _playSceneContentProvider));

        _systemManager.AddSystem(
            new PlayerInputSystem(
                _messenger,
                playContext));

        _systemManager.AddSystem(
            new RenderSystem(
                _entityContext,
                _spriteBatch));

        _systemManager.AddSystem(
            new DiagnosticSystem(
                _spriteBatch,
                _playSceneContentProvider));
    }

    public void Clear()
    {
        _systemManager.ClearSystems();
    }

    public void Update(GameTime gameTime)
    {
        _systemManager.Update(gameTime);
    }

    public void Draw(GameTime gameTime)
    {
        _systemManager.Draw(gameTime);
    }
}
