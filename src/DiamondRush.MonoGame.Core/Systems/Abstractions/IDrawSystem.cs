using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Core.Systems.Abstractions;

public interface IDrawSystem :
    ISystem
{
    public void Draw(GameTime gameTime);
}
