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

        var allGemsAttachedToGameBoard = true;

        foreach (var gemEntity in _gemEntityView.AsEnumerable())
        {
            var gemPlayBehavior = _gemPlayBehaviorStore.Get(gemEntity);

            if (!gemPlayBehavior.IsFalling)
            {
                continue;
            }

            var gemRectTransform = _rectTransformStore.Get(gemEntity);

            var targetGameBoardFieldPosition = gemPlayBehavior
                .TargetGameBoardFieldPosition!.Value;

            var newGemRectTransform = MoveGemToTargetGameBoardField(
                gemEntity,
                gemRectTransform,
                targetGameBoardFieldPosition,
                deltaTime);

            if (!TryAttachGemToGameBoardField(
                    gemEntity,
                    gemPlayBehavior,
                    newGemRectTransform,
                    targetGameBoardFieldPosition))
            {
                allGemsAttachedToGameBoard = false;
            }
        }

        if (allGemsAttachedToGameBoard)
        {
            _playContext.SetPlayState(PlayState.WaitingForInput);
        }
    }

    private bool IsUpdateEnabled() =>
        _playContext.PlayState == PlayState.FallingGems;

    private RectTransform MoveGemToTargetGameBoardField(
        Entity gemEntity,
        RectTransform gemRectTransform,
        Vector2 targetGameBoardFieldPosition,
        float deltaTime)
    {
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
        GemPlayBehavior gemPlayBehavior,
        RectTransform gemRectTransform,
        RectTransform gameBoardRectTransform)
    {
        if (gemRectTransform.IsInside(gameBoardRectTransform))
        {
            _gemPlayBehaviorStore.Set(
                gemEntity,
                gemPlayBehavior.SetVisibility(true));
        }
    }

    private bool TryAttachGemToGameBoardField(
        Entity gemEntity,
        GemPlayBehavior gemPlayBehavior,
        RectTransform gemRectTransform,
        Vector2 targetGameBoardFieldPosition)
    {
        if (gemRectTransform.Position != targetGameBoardFieldPosition)
        {
            return false;
        }

        gemPlayBehavior
            .TargetGameBoardField!
            .AttachGem(gemEntity);

        _gemPlayBehaviorStore.Set(
            gemEntity,
            gemPlayBehavior.FinishFallingToGameBoardField());

        return true;
    }
}
