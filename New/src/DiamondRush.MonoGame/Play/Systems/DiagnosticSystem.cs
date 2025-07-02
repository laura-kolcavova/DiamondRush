using DiamondRush.MonoGame.Core.Systems;
using DiamondRush.MonoGame.Core.Tools;
using DiamondRush.MonoGame.Play.Content.Abstractions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DiamondRush.MonoGame.Play.Systems;

internal sealed class DiagnosticSystem :
    IUpdateSystem,
    IDrawSystem
{
    private readonly IPlaySceneContentProvider _playSceneContentProvider;

    private readonly SpriteBatch _spriteBatch;

    private readonly FrameCounter _frameCounter;

    public DiagnosticSystem(
        SpriteBatch spriteBatch,
        IPlaySceneContentProvider playSceneContentProvider)
    {
        _playSceneContentProvider = playSceneContentProvider;

        _spriteBatch = spriteBatch;

        _frameCounter = new FrameCounter();
    }

    public void Update(GameTime gameTime)
    {
        _frameCounter.Update(gameTime);
    }

    public void Draw(GameTime gameTime)
    {
        _spriteBatch.DrawString(
            _playSceneContentProvider.DefaultFont,
            $"FPS: {_frameCounter.FramesPerSecond}",
            new Vector2(10, 10),
            Color.Black);
    }
}
