using DiamondRush.MonoGame.Core.Scenes.Abstractions;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Core.Scenes;

public class Scene : IScene
{
    protected bool IsDisposed;

    public string Name { get; }

    public bool IsLoaded { get; private set; }

    protected Scene(
        string name)
    {
        Name = !string.IsNullOrEmpty(name)
            ? name
            : throw new ArgumentException(
                "Scene name cannot be null or empty.",
                nameof(name));
    }

    ~Scene()
    {
        Dispose(false);
    }

    public void Load()
    {
        if (IsLoaded)
        {
            return;
        }

        Initialize();

        IsLoaded = true;
    }

    public void Unload()
    {
        if (!IsLoaded)
        {
            return;
        }

        UnloadContent();

        IsLoaded = false;
    }

    public virtual void Update(
        GameTime gameTime)
    {
    }

    public virtual void Draw(
        GameTime gameTime)
    {
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Initialize()
    {
        LoadContent();
    }

    protected virtual void LoadContent()
    {
    }

    protected virtual void UnloadContent()
    {
    }

    protected virtual void OnDisposing()
    {
    }

    protected virtual void OnDisposed()
    {
    }

    protected virtual void Dispose(
        bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        if (disposing)
        {
            OnDisposing();
        }

        OnDisposed();

        IsDisposed = true;
    }
}
