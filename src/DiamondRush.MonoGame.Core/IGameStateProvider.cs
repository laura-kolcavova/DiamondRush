using Microsoft.Xna.Framework.Graphics;

namespace DiamondRush.MonoGame.Core;

public interface IGameStateProvider
{
    public bool IsActive { get; }

    public GraphicsDevice GraphicsDevice { get; }

    public IServiceProvider Services { get; }
}
