namespace DiamondRush.MonoGame.Core.Scenes.Abstractions;

public interface ISceneManager
{
    public void SetActiveScene(IScene scene);

    public IScene? GetActiveScene();
}
