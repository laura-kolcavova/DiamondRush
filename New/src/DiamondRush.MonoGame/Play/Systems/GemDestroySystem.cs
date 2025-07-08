using DiamondRush.MonoGame.Core.Systems;
using DiamondRush.MonoGame.Play.Components;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play.Systems;

internal sealed class GemDestroySystem :
    IUpdateSystem
{
    private readonly IEntityContext _entityContext;

    private readonly IEntityView _gemEntityView;

    private readonly IComponentStore<GemPlayBehavior> _gemPlayBehaviorStore;

    public GemDestroySystem(
        IEntityContext entityContext)
    {
        _entityContext = entityContext;

        _gemEntityView = entityContext
            .UseQuery()
            .With<Gem>()
            .With<GemPlayBehavior>()
            .AsView();

        _gemPlayBehaviorStore = entityContext.UseStore<GemPlayBehavior>();
    }

    public void Update(GameTime gameTime)
    {
        foreach (var gemEntity in _gemEntityView.AsEnumerable())
        {
            var gemPlayBehavior = _gemPlayBehaviorStore.Get(gemEntity);

            if (gemPlayBehavior.IsCollected)
            {
                _entityContext.DestroyEntity(gemEntity);
            }
        }
    }
}
