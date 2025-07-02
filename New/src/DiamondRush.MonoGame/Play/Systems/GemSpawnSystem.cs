using DiamondRush.MonoGame.Core.Systems;
using DiamondRush.MonoGame.Play.Content.Abstractions;
using DiamondRush.MonoGame.Play.Factories;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play.Systems;

internal sealed class GemSpawnSystem :
    IUpdateSystem
{
    private readonly IEntityContext _entityContext;

    private readonly GemEntityFactory _gemEntityFactory;

    private int _counter = 1;

    public GemSpawnSystem(
        IEntityContext entityContext,
        IPlaySceneContentProvider playSceneContentProvider)
    {
        _entityContext = entityContext;

        _gemEntityFactory = new GemEntityFactory(playSceneContentProvider);
    }

    public void Update(GameTime gameTime)
    {
        if (_counter > 0)
        {
            _gemEntityFactory.Create(_entityContext, GemType.Red, new Vector2(50, 50));
            _counter--;
        }
    }
}
