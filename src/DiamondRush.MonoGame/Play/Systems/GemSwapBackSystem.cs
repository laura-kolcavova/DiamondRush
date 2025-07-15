using DiamondRush.MonoGame.Core;
using DiamondRush.MonoGame.Core.Movement;
using DiamondRush.MonoGame.Core.Systems.Abstractions;
using DiamondRush.MonoGame.Play.Components;
using LightECS;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;


namespace DiamondRush.MonoGame.Play.Systems;

internal sealed class GemSwapBackSystem
    : IUpdateSystem
{
    private readonly PlayContext _playContext;

    private readonly IEntityView _gemEntityView;

    private readonly IComponentStore<GemPlayBehavior> _gemPlayBehaviorStore;

    private readonly IComponentStore<RectTransform> _rectTransformStore;

    public GemSwapBackSystem(
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

        var allGemsSwappedBack = true;

        foreach (var gemEntity in _gemEntityView.AsEnumerable())
        {
            var gemPlayBehavior = _gemPlayBehaviorStore.Get(gemEntity);

            if (!gemPlayBehavior.IsSwappingBack)
            {
                continue;
            }

            var originalGameBoardField = _playContext
               .GameBoardFields
               .GetField(
                   gemPlayBehavior.OriginalRowIndex,
                   gemPlayBehavior.OriginalColumnIndex);

            if (!ProcessSwapBack(
                gemEntity,
                originalGameBoardField,
                deltaTime))
            {
                allGemsSwappedBack = false;

                continue;
            }

            FinishSwapBack(
                gemEntity,
                originalGameBoardField);
        }

        if (allGemsSwappedBack)
        {
            _playContext.SetPlayState(PlayState.WaitingForInput);
        }
    }

    private bool IsUpdateEnabled() =>
       _playContext.PlayState == PlayState.SwappingGemsBack;

    private bool ProcessSwapBack(
      Entity gemEntity,
      GameBoardField originalGameBoardField,
      float deltaTime)
    {
        var originalGameBoardFieldPosition = _playContext.GetGameBoardFieldPosition(
           originalGameBoardField);

        var gemRectTransform = _rectTransformStore.Get(gemEntity);

        var newGemPosition = gemRectTransform
            .Position
            .MoveTowards(
                originalGameBoardFieldPosition,
                Constants.GemSwapSpeed * deltaTime);

        _rectTransformStore.Set(
            gemEntity,
            gemRectTransform.UpdatePosition(newGemPosition));

        return newGemPosition == originalGameBoardFieldPosition;
    }

    private void FinishSwapBack(
        Entity gemEntity,
        GameBoardField originalGameBoardField)
    {
        originalGameBoardField.AttachGem(gemEntity);

        var gemPlayBehavior = _gemPlayBehaviorStore.Get(gemEntity);

        _gemPlayBehaviorStore.Set(
            gemEntity,
            gemPlayBehavior.FinishSwappingBack());
    }

}
