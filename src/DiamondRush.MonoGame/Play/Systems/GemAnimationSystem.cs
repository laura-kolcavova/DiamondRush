using DiamondRush.MonoGame.Core.Systems.Abstractions;
using DiamondRush.MonoGame.Play.Components;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play.Systems;

internal sealed class GemAnimationSystem
    : IUpdateSystem
{
    private readonly IEntityView _gemEntityView;

    private readonly IComponentStore<GemPlayBehavior> _gemPlayBehaviorStore;

    public GemAnimationSystem(
        IEntityContext entityContext,
        IEntityView gemEntityView)
    {
        _gemEntityView = gemEntityView;

        _gemPlayBehaviorStore = entityContext.UseStore<GemPlayBehavior>();
    }

    public void Update(GameTime gameTime)
    {
        foreach (var gemEntity in _gemEntityView.AsEnumerable())
        {
            var gemPlayBehavior = _gemPlayBehaviorStore.Get(gemEntity);

            if (!gemPlayBehavior.CollectAnimationEnabled)
            {
                continue;
            }

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var step = deltaTime / Constants.Gem.CollectAnimationDurationInSeconds;

            var newCollectAnimationProgress = gemPlayBehavior.CollectAnimationProgress + step;

            _gemPlayBehaviorStore.Set(
                   gemEntity,
                   gemPlayBehavior.UpdateCollectAnimationProgress(newCollectAnimationProgress));
        }
    }
}
