using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Core.Systems.Abstractions;

public interface IUpdateSystem :
    ISystem
{
    public void Update(GameTime gameTime);
}
