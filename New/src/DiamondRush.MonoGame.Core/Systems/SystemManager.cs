using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Core.Systems;

public sealed class SystemManager
{
    private readonly List<IUpdateSystem> _updateSystems;

    private readonly List<IDrawSystem> _drawSystems;

    public SystemManager()
    {
        _updateSystems = [];
        _drawSystems = [];
    }

    public SystemManager AddSystem(ISystem system)
    {
        if (system is IUpdateSystem updateSystem)
        {
            _updateSystems.Add(updateSystem);
        }

        if (system is IDrawSystem drawSystem)
        {
            _drawSystems.Add(drawSystem);
        }

        return this;
    }

    public void Update(GameTime gameTime)
    {
        foreach (var system in _updateSystems)
        {
            system.Update(gameTime);
        }
    }

    public void Draw(GameTime gameTime)
    {
        foreach (var system in _drawSystems)
        {
            system.Draw(gameTime);
        }
    }

    public void ClearSystems()
    {
        _updateSystems.Clear();
        _drawSystems.Clear();
    }
}
