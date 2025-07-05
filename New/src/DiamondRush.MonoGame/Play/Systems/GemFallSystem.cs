using DiamondRush.MonoGame.Core;
using DiamondRush.MonoGame.Core.Movement;
using DiamondRush.MonoGame.Core.Systems;
using DiamondRush.MonoGame.Play.Components;
using LightECS;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play.Systems;

internal sealed class GemFallSystem :
    IUpdateSystem
{
    private readonly PlayContext _playContext;

    private readonly IEntityView _gemEntityView;

    private readonly IComponentStore<RectTransform> _rectTransformStore;

    private readonly IComponentStore<GemPlayBehavior> _gemPlayBehaviorStore;

    public GemFallSystem(
        IEntityContext entityContext,
        PlayContext playContext)
    {
        _playContext = playContext;

        _gemEntityView = entityContext
            .UseQuery()
            .With<Gem>()
            .With<RectTransform>()
            .With<GemPlayBehavior>()
            .AsView();

        _rectTransformStore = entityContext.UseStore<RectTransform>();
        _gemPlayBehaviorStore = entityContext.UseStore<GemPlayBehavior>();
    }

    public void Update(GameTime gameTime)
    {
        if (!IsUpdateEnabled())
        {
            return;
        }

        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (TryFallAndAttachGems(deltaTime))
        {
            _playContext.SetPlayState(PlayState.MatchingGems);
        }
    }

    private bool IsUpdateEnabled() =>
        _playContext.PlayState == PlayState.FallingGems;

    private bool TryFallAndAttachGems(
        float deltaTime)
    {
        var allGemsAttached = true;

        var gameBoardRectTransform = _rectTransformStore.Get(
            _playContext.GameBoardEntity);

        foreach (var gemEntity in _gemEntityView.AsEnumerable())
        {
            var gemPlayBehavior = _gemPlayBehaviorStore.Get(gemEntity);

            if (!gemPlayBehavior.IsFalling)
            {
                continue;
            }

            var targetGameBoardFieldPosition = gemPlayBehavior
                .TargetGameBoardFieldPosition!.Value;

            MoveGemToTargetGameBoardField(
                gemEntity,
                targetGameBoardFieldPosition,
                deltaTime);

            TryUpdateGemVisibility(
                gemEntity,
                gameBoardRectTransform);

            if (!TryAttachGemToGameBoardField(
                gemEntity,
                targetGameBoardFieldPosition))
            {
                allGemsAttached = false;
            }
        }

        return allGemsAttached;
    }

    private RectTransform MoveGemToTargetGameBoardField(
        Entity gemEntity,
        Vector2 targetGameBoardFieldPosition,
        float deltaTime)
    {
        var gemRectTransform = _rectTransformStore.Get(gemEntity);

        var newGemPosition = gemRectTransform.Position.MoveTowards(
            targetGameBoardFieldPosition,
            Constants.GemFallSpeed * deltaTime);

        var newGemRectTransform = gemRectTransform.UpdatePosition(
            newGemPosition);

        _rectTransformStore.Set(
            gemEntity,
            newGemRectTransform);

        return newGemRectTransform;
    }

    private void TryUpdateGemVisibility(
        Entity gemEntity,
        RectTransform gameBoardRectTransform)
    {
        var gemPlayBehavior = _gemPlayBehaviorStore.Get(gemEntity);

        if (gemPlayBehavior.IsVisible)
        {
            return;
        }

        var gemRectTransform = _rectTransformStore.Get(gemEntity);

        if (gemRectTransform.IsInside(gameBoardRectTransform))
        {
            _gemPlayBehaviorStore.Set(
                gemEntity,
                gemPlayBehavior.SetVisibility(true));
        }
    }

    private bool TryAttachGemToGameBoardField(
        Entity gemEntity,
        Vector2 targetGameBoardFieldPosition)
    {
        var gemRectTransform = _rectTransformStore.Get(gemEntity);

        if (gemRectTransform.Position != targetGameBoardFieldPosition)
        {
            return false;
        }

        var gemPlayBehavior = _gemPlayBehaviorStore.Get(gemEntity);

        gemPlayBehavior
            .TargetGameBoardField!
            .AttachGem(gemEntity);

        _gemPlayBehaviorStore.Set(
            gemEntity,
            gemPlayBehavior.FinishFallingToGameBoardField());

        return true;
    }
}
