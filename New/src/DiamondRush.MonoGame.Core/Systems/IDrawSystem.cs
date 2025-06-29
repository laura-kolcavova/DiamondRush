using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Core.Systems;

public interface IDrawSystem :
    ISystem
{
    public void Draw(GameTime gameTime);
}
