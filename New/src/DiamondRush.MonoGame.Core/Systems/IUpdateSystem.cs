using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Core.Systems;

public interface IUpdateSystem :
    ISystem
{
    public void Update(GameTime gameTime);
}
