using DiamondRush.MonoGame.Core.Scenes.Abstractions;

namespace DiamondRush.MonoGame.Core.Scenes;

public sealed class SceneManager :
    ISceneManager
{
    private IScene? _activeScene;

    public IScene GetActiveScene()
    {
        return _activeScene
            ?? throw new InvalidOperationException("No active scene is set.");
    }

    public void SetActiveScene(
        IScene scene)
    {
        if (!scene.IsLoaded)
        {
            throw new InvalidOperationException("The scene must be loaded before it can be set as active.");
        }

        _activeScene = scene;
    }
}
