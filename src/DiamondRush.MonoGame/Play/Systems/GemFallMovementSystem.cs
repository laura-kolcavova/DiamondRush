using DiamondRush.MonoGame.Core;
using DiamondRush.MonoGame.Core.Movement;
using DiamondRush.MonoGame.Core.Systems.Abstractions;
using DiamondRush.MonoGame.Play.Components;
using LightECS;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play.Systems;

internal sealed class GemFallMovementSystem :
    IUpdateSystem
{
    private readonly PlayContext _playContext;

    private readonly IEntityView _gemEntityView;

    private readonly IComponentStore<RectTransform> _rectTransformStore;

    private readonly IComponentStore<GemPlayBehavior> _gemPlayBehaviorStore;

    public GemFallMovementSystem(
        IEntityContext entityContext,
        PlayContext playContext,
        IEntityView gemEntityView)
    {
        _playContext = playContext;

        _gemEntityView = gemEntityView;

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
            _playContext.SetPlayState(PlayState.GemMatch);
        }
    }

    private bool IsUpdateEnabled() =>
        _playContext.PlayState == PlayState.GemFallMovement;

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

            var targetGameBoardField = _playContext.GameBoardFields.GetField(
                gemPlayBehavior.TargetRowIndex,
                gemPlayBehavior.TargetColumnIndex);

            var targetGameBoardFieldPosition = _playContext.GetGameBoardFieldPosition(
                targetGameBoardField);

            MoveGemToTargetGameBoardField(
                gemEntity,
                targetGameBoardFieldPosition,
                deltaTime);

            UpdateGemVisibility(
                gemEntity,
                gameBoardRectTransform);

            if (!TryAttachGemToGameBoardField(
                gemEntity,
                targetGameBoardField,
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
            Constants.Gem.FallSpeed * deltaTime);

        var newGemRectTransform = gemRectTransform.UpdatePosition(
            newGemPosition);

        _rectTransformStore.Set(
            gemEntity,
            newGemRectTransform);

        return newGemRectTransform;
    }

    private void UpdateGemVisibility(
        Entity gemEntity,
        RectTransform gameBoardRectTransform)
    {
        var gemPlayBehavior = _gemPlayBehaviorStore.Get(gemEntity);

        if (gemPlayBehavior.IsVisible)
        {
            return;
        }

        var gemRectTransform = _rectTransformStore.Get(gemEntity);

        if (gameBoardRectTransform.Contains(gemRectTransform))
        {
            _gemPlayBehaviorStore.Set(
                gemEntity,
                gemPlayBehavior.SetVisibility(true));
        }
    }

    private bool TryAttachGemToGameBoardField(
        Entity gemEntity,
        GameBoardField gameBoardField,
        Vector2 targetGameBoardFieldPosition)
    {
        var gemRectTransform = _rectTransformStore.Get(gemEntity);

        if (gemRectTransform.Position != targetGameBoardFieldPosition)
        {
            return false;
        }

        var gemPlayBehavior = _gemPlayBehaviorStore.Get(gemEntity);

        gameBoardField
            .AttachGem(gemEntity);

        _gemPlayBehaviorStore.Set(
            gemEntity,
            gemPlayBehavior.FinishFalling());

        return true;
    }
}
