using DiamondRush.MonoGame.Core.Systems.Abstractions;
using DiamondRush.MonoGame.Play.Components;
using LightECS;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play.Systems;

internal sealed class GemFallInitiationSystem :
    IUpdateSystem
{
    private readonly PlayContext _playContext;

    private readonly IComponentStore<GemPlayBehavior> _gemPlayBehaviorStore;

    public GemFallInitiationSystem(
        IEntityContext entityContext,
        PlayContext playContext)
    {
        _playContext = playContext;

        _gemPlayBehaviorStore = entityContext.UseStore<GemPlayBehavior>();
    }

    public void Update(GameTime gameTime)
    {
        if (!IsUpdateEnabled())
        {
            return;
        }

        if (StartFallingGems())
        {
            _playContext.ClearSpawnedGemEntities();

            _playContext.SetPlayState(PlayState.GemFallMovement);
        }
        else
        {
            _playContext.SetPlayState(PlayState.WaitingForInput);
        }
    }

    private bool IsUpdateEnabled() =>
        _playContext.PlayState == PlayState.GemFallInitiation;

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
            gemPlayBehavior.StartFalling(
                targetRowIndex,
                targetColumnIndex));
    }
}
