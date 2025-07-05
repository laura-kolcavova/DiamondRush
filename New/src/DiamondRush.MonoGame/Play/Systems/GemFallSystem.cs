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
    private readonly IEntityContext _entityContext;

    private readonly PlayContext _playContext;

    private readonly IComponentStore<RectTransform> _rectTransformStore;

    private readonly IComponentStore<GemPlayBehavior> _gemPlayBehaviorStore;

    private readonly IEntityView _fallingGemsView;

    public GemFallSystem(
        IEntityContext entityContext,
        PlayContext playContext)
    {
        _entityContext = entityContext;
        _playContext = playContext;

        _rectTransformStore = _entityContext.UseStore<RectTransform>();
        _gemPlayBehaviorStore = _entityContext.UseStore<GemPlayBehavior>();

        _fallingGemsView = _entityContext
            .UseQuery()
            .With<Gem>()
            .With<RectTransform>()
            .With<GemPlayBehavior>()
            .AsView();
    }
    public void Update(GameTime gameTime)
    {
        if (!IsUpdateEnabled())
        {
            return;
        }

        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        var allGemsAttachedToGameBoard = true;

        foreach (var gemEntity in _fallingGemsView.AsEnumerable())
        {
            var gemPlayBehavior = _gemPlayBehaviorStore.Get(gemEntity);

            if (!gemPlayBehavior.IsFalling)
            {
                continue;
            }

            var gemRectTransform = _rectTransformStore.Get(gemEntity);

            if (MoveGemToTargetGameBoardField(
                gemEntity,
                gemRectTransform,
                gemPlayBehavior.TargetGameBoardFieldPosition!.Value,
                deltaTime))
            {
                AttachGemToGameBoardField(
                    gemEntity,
                    gemPlayBehavior);
            }
            else
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

    private bool MoveGemToTargetGameBoardField(
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

        if (newGemPosition == targetGameBoardFieldPosition)
        {
            return true;
        }

        return false;
    }

    private void AttachGemToGameBoardField(
        Entity gemEntity,
        GemPlayBehavior gemPlayBehavior)
    {
        gemPlayBehavior
            .TargetGameBoardField!
            .AttachGem(gemEntity);

        _gemPlayBehaviorStore.Set(
            gemEntity,
            gemPlayBehavior.FinishFallingToGameBoardField());
    }
}
