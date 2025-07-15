using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Core.Scenes.Abstractions;

public interface IScene : IDisposable
{
    public string Name { get; }

    public bool IsLoaded { get; }

    public void Load();

    public void Unload();

    public void Update(
        GameTime gameTime);

    public void Draw(
        GameTime gameTime);
}
