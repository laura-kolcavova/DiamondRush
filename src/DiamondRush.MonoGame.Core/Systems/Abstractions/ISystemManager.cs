using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Core.Systems.Abstractions;

public interface ISystemManager
{
    public SystemManager AddSystem(
        ISystem system);

    public void Update(
        GameTime gameTime);

    public void Draw(
        GameTime gameTime);

    public void ClearSystems();
}
