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

    private bool _startFallingGemsFinished = false;

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

        if (!_startFallingGemsFinished)
        {
            if (StartFallingGems())
            {
                _playContext.ClearSpawnedGemEntities();

                _startFallingGemsFinished = true;
            }
            else
            {
                _playContext.SetPlayState(PlayState.WaitingForInput);

                _startFallingGemsFinished = false;

                return;
            }
        }

        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (TryFallAndAttachGems(deltaTime))
        {
            _playContext.SetPlayState(PlayState.MatchingGems);

            _startFallingGemsFinished = false;
        }
    }

    private bool IsUpdateEnabled() =>
        _playContext.PlayState == PlayState.FallingGems;

    private bool StartFallingGems()
    {
        var anyGemIsFalling = false;

        for (var columnIndex = 0; columnIndex < _playContext.GameBoardFields.Columns; columnIndex++)
        {
            if (StartFallingGemsInColumn(columnIndex))
            {
                anyGemIsFalling = true;
            }
        }

        return anyGemIsFalling;
    }

    private bool StartFallingGemsInColumn(
        int columnIndex)
    {
        var anyGemIsFalling = false;

        var lowestEmptyGameBoardField = _playContext
            .GameBoardFields
            .FieldsInColumn(columnIndex)
            .LastOrDefault(gameBoardField => gameBoardField.IsEmpty);

        if (lowestEmptyGameBoardField is null)
        {
            return false;
        }

        var nextTargetRowIndex = lowestEmptyGameBoardField.RowIndex;

        if (StartFallingGemsInGameBoardColumn(
            columnIndex,
            lowestEmptyGameBoardField.RowIndex,
            ref nextTargetRowIndex))
        {
            anyGemIsFalling = true;
        }

        if (StartFallingSpawnedGems(
            columnIndex,
            ref nextTargetRowIndex))
        {
            anyGemIsFalling = true;
        }

        return anyGemIsFalling;
    }

    private bool StartFallingGemsInGameBoardColumn(
        int columnIndex,
        int lowestEmptyGameBoardFieldRowIndex,
        ref int nextTargetRowIndex)
    {
        if (lowestEmptyGameBoardFieldRowIndex == 0)
        {
            return false;
        }

        var anyGemIsFalling = false;

        var startRowIndex = lowestEmptyGameBoardFieldRowIndex - 1;

        for (var rowIndex = startRowIndex; rowIndex >= 0; rowIndex--)
        {
            var gameBoardFieldToDetach = _playContext
                .GameBoardFields
                .GetField(
                    rowIndex,
                    columnIndex);

            if (gameBoardFieldToDetach.IsEmpty)
            {
                continue;
            }

            var detachedGemEntity = gameBoardFieldToDetach.GemEntity;

            gameBoardFieldToDetach.DetachGem();

            StartFallingGemEntity(
                detachedGemEntity,
                nextTargetRowIndex,
                columnIndex);

            nextTargetRowIndex--;

            anyGemIsFalling = true;
        }

        return anyGemIsFalling;
    }

    private bool StartFallingSpawnedGems(
        int columnIndex,
        ref int nextTargetRowIndex)
    {
        if (!_playContext.TryGetSpawnedGemEntities(
            columnIndex,
            out var spawnedGemEntityStack))
        {
            return false;
        }

        foreach (var spawnedGemEntity in spawnedGemEntityStack)
        {
            StartFallingGemEntity(
              spawnedGemEntity,
              nextTargetRowIndex,
              columnIndex);

            nextTargetRowIndex--;
        }

        return true;
    }

    private void StartFallingGemEntity(
        Entity gemEntity,
        int targetRowIndex,
        int targetColumnIndex)
    {
        var gemPlayBehavior = _gemPlayBehaviorStore.Get(
            gemEntity);

        _gemPlayBehaviorStore.Set(
            gemEntity,
            gemPlayBehavior.StartFallingToGameBoardField(
                targetRowIndex,
                targetColumnIndex));
    }

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
            Constants.GemFallSpeed * deltaTime);

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

        if (gemRectTransform.IsInside(gameBoardRectTransform))
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
            gemPlayBehavior.FinishFallingToGameBoardField());

        return true;
    }
}
