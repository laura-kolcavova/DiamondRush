using DiamondRush.MonoGame.Core.Systems;
using DiamondRush.MonoGame.Play.Components;
using LightECS;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play.Systems;

internal sealed class GemCollectSystem :
    IUpdateSystem
{
    private readonly PlayContext _playContext;

    private readonly IEntityView _gemEntityView;

    private readonly IComponentStore<GemPlayBehavior> _gemPlayBehaviorStore;

    public GemCollectSystem(
        IEntityContext entityContext,
        PlayContext playContext)
    {
        _playContext = playContext;

        _gemEntityView = entityContext
            .UseQuery()
            .With<Gem>()
            .With<GemPlayBehavior>()
            .AsView();

        _gemPlayBehaviorStore = entityContext.UseStore<GemPlayBehavior>();
    }

    public void Update(GameTime gameTime)
    {
        if (!IsUpdateEnabled())
        {
            return;
        }

        foreach (var gemEntity in _gemEntityView.AsEnumerable())
        {
            var gemPlayBehavior = _gemPlayBehaviorStore.Get(gemEntity);

            TryMarkGemAsCollecting(gemEntity);

            TryCollectGem(gemEntity);
        }
    }

    private bool IsUpdateEnabled() =>
        _playContext.PlayState == PlayState.CollectingGems;

    private void TryMarkGemAsCollecting(
        Entity gemEntity)
    {
        var gemPlayBehavior = _gemPlayBehaviorStore.Get(gemEntity);

        if (gemPlayBehavior.IsCollecting || gemPlayBehavior.IsCollected)
        {
            return;
        }

        if (gemPlayBehavior.IsMatching)
        {
            _gemPlayBehaviorStore.Set(
               gemEntity,
               gemPlayBehavior
               .StartCollecting());
        }
    }

    private void TryCollectGem(
        Entity gemEntity)
    {
        var gemPlayBehavior = _gemPlayBehaviorStore.Get(gemEntity);

        if (gemPlayBehavior.IsCollected)
        {
            return;
        }

        if (gemPlayBehavior.IsCollecting)
        {
            _gemPlayBehaviorStore.Set(
               gemEntity,
               gemPlayBehavior
               .FinishCollecting()
               .SetVisibility(false));
        }
    }
}
