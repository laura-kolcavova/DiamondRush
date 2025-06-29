using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Core.Tools;

public sealed class FrameCounter
{
    private int _frameCount = 0;

    private double _elapsedTime = 0;

    public int FramesPerSecond { get; private set; }

    public void Update(GameTime gameTime)
    {
        _elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
        _frameCount++;

        if (_elapsedTime >= 1.0)
        {
            FramesPerSecond = _frameCount;

            _elapsedTime = 0;
            _frameCount = 0;
        }

        //FramesPerSecond = (int)((float)1 / (float)gameTime.ElapsedGameTime.TotalSeconds);
    }
}
