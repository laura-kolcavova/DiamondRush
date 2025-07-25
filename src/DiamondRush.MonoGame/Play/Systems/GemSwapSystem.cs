﻿using DiamondRush.MonoGame.Core;
using DiamondRush.MonoGame.Core.Movement;
using DiamondRush.MonoGame.Core.Systems.Abstractions;
using DiamondRush.MonoGame.Play.Components;
using LightECS;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play.Systems;

internal sealed class GemSwapSystem :
    IUpdateSystem
{
    private readonly PlayContext _playContext;

    private readonly IEntityView _gemEntityView;

    private readonly IComponentStore<GemPlayBehavior> _gemPlayBehaviorStore;

    private readonly IComponentStore<RectTransform> _rectTransformStore;

    public GemSwapSystem(
        IEntityContext entityContext,
        PlayContext playContext,
        IEntityView gemEntityView)
    {
        _playContext = playContext;

        _gemEntityView = gemEntityView;

        _gemPlayBehaviorStore = entityContext.UseStore<GemPlayBehavior>();

        _rectTransformStore = entityContext.UseStore<RectTransform>();
    }

    public void Update(GameTime gameTime)
    {
        if (!IsUpdateEnabled())
        {
            return;
        }

        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        var allGemsSwapped = true;

        foreach (var gemEntity in _gemEntityView.AsEnumerable())
        {
            var gemPlayBehavior = _gemPlayBehaviorStore.Get(gemEntity);

            if (!gemPlayBehavior.IsSwapping)
            {
                continue;
            }

            var targetGameBoardField = _playContext
               .GameBoardFields
               .GetField(
                   gemPlayBehavior.TargetRowIndex,
                   gemPlayBehavior.TargetColumnIndex);

            if (!SwapGemToTargetGameBoardField(
                gemEntity,
                targetGameBoardField,
                deltaTime))
            {
                allGemsSwapped = false;

                continue;
            }

            FinishGemSwapping(
                gemEntity,
                targetGameBoardField);
        }

        if (allGemsSwapped)
        {
            _playContext.SetPlayState(PlayState.GemMatch);
        }
    }

    private bool IsUpdateEnabled() =>
       _playContext.PlayState == PlayState.GemSwap;

    private bool SwapGemToTargetGameBoardField(
        Entity gemEntity,
        GameBoardField targetGameBoardField,
        float deltaTime)
    {
        var targetGameBoardFieldPosition = _playContext.GetGameBoardFieldPosition(
           targetGameBoardField);

        var gemRectTransform = _rectTransformStore.Get(gemEntity);

        var newGemPosition = gemRectTransform
            .Position
            .MoveTowards(
                targetGameBoardFieldPosition,
                Constants.Gem.SwapSpeed * deltaTime);

        _rectTransformStore.Set(
            gemEntity,
            gemRectTransform.UpdatePosition(newGemPosition));

        return newGemPosition == targetGameBoardFieldPosition;
    }

    private void FinishGemSwapping(
        Entity gemEntity,
        GameBoardField targetGameBoardField)
    {
        targetGameBoardField.AttachGem(gemEntity);

        var gemPlayBehavior = _gemPlayBehaviorStore.Get(gemEntity);

        _gemPlayBehaviorStore.Set(
            gemEntity,
            gemPlayBehavior.FinishSwapping());
    }
}
