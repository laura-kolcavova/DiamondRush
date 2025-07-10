using DiamondRush.MonoGame.Core.Systems;
using DiamondRush.MonoGame.Play.Components;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play.Systems;

internal sealed class GemCollectSystem :
    IUpdateSystem
{
    private readonly PlayContext _playContext;

    private readonly IEntityView _gemEntityView;

    private readonly IComponentStore<GemPlayBehavior> _gemPlayBehaviorStore;

    private bool _startCollectMatchingGemsFinished = false;

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

        if (!_startCollectMatchingGemsFinished)
        {
            if (StartCollectingMatchingGems())
            {
                _startCollectMatchingGemsFinished = true;
            }
            else
            {
                _playContext.SetPlayState(PlayState.WaitingForInput);

                _startCollectMatchingGemsFinished = false;
            }
        }
        else
        {
            if (FinishCollectingGems())
            {
                _playContext.SetPlayState(PlayState.SpawningNewGems);

                _startCollectMatchingGemsFinished = false;
            }
        }
    }

    private bool IsUpdateEnabled() =>
        _playContext.PlayState == PlayState.CollectingGems;

    private bool StartCollectingMatchingGems()
    {
        var anyGemIsCollecting = false;

        foreach (var gemEntity in _gemEntityView.AsEnumerable())
        {
            var gemPlayBehavior = _gemPlayBehaviorStore.Get(gemEntity);

            if (!gemPlayBehavior.IsMatching)
            {
                continue;
            }

            _gemPlayBehaviorStore.Set(
                gemEntity,
                gemPlayBehavior
                .StartCollecting());

            anyGemIsCollecting = true;
        }

        return anyGemIsCollecting;
    }

    private bool FinishCollectingGems()
    {
        var allGemsCollected = true;

        foreach (var gemEntity in _gemEntityView.AsEnumerable())
        {
            var gemPlayBehavior = _gemPlayBehaviorStore.Get(gemEntity);

            if (!(gemPlayBehavior.IsMatching && gemPlayBehavior.IsCollecting))
            {
                continue;
            }

            if (gemPlayBehavior.CollectAnimationProgress < 1f)
            {
                allGemsCollected = false;

                continue;
            }

            _gemPlayBehaviorStore.Set(
                gemEntity,
                gemPlayBehavior
                .FinishCollecting()
                .SetVisibility(false));

            var attachedGameBoardField = _playContext
                .GameBoardFields
                .GetField(
                    gemPlayBehavior.AttachedToRowIndex,
                    gemPlayBehavior.AttachedColumnIndex);

            attachedGameBoardField.DetachGem();
        }

        return allGemsCollected;
    }
}
