﻿using DiamondRush.MonoGame.Core.Systems.Abstractions;
using DiamondRush.MonoGame.Play.Components;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play.Systems;

internal sealed class GemMatchSystem :
    IUpdateSystem
{
    private readonly PlayContext _playContext;

    private readonly IComponentStore<Gem> _gemStore;

    private readonly IEntityView _gemEntityView;

    private readonly IComponentStore<GemPlayBehavior> _gemPlayBehaviorStore;

    public GemMatchSystem(
        IEntityContext entityContext,
        PlayContext playContext,
        IEntityView entityView)
    {
        _playContext = playContext;

        _gemStore = entityContext.UseStore<Gem>();

        _gemEntityView = entityView;

        _gemPlayBehaviorStore = entityContext.UseStore<GemPlayBehavior>();
    }

    public void Update(GameTime gameTime)
    {
        if (!IsUpdateEnabled())
        {
            return;
        }

        var anyGemIsMatching = TrySearchForMatchingGems();

        if (anyGemIsMatching)
        {
            foreach (var gemEntity in _gemEntityView.AsEnumerable())
            {
                var gemPlayBehavior = _gemPlayBehaviorStore.Get(gemEntity);

                if (gemPlayBehavior.IsSwapped)
                {
                    _gemPlayBehaviorStore.Set(
                        gemEntity,
                        gemPlayBehavior.ConfirmSwappedPosition());
                }
            }

            _playContext.SetPlayState(PlayState.GemCollect);
        }
        else if (_playContext.PreviousPlayState == PlayState.GemSwap)
        {
            foreach (var gemEntity in _gemEntityView.AsEnumerable())
            {
                var gemPlayBehavior = _gemPlayBehaviorStore.Get(gemEntity);

                if (gemPlayBehavior.IsSwapped)
                {
                    var currentGameBoardField = _playContext
                       .GameBoardFields
                       .GetField(
                           gemPlayBehavior.CurrentRowIndex,
                           gemPlayBehavior.CurrentColumnIndex);

                    currentGameBoardField.DetachGem();

                    _gemPlayBehaviorStore.Set(
                        gemEntity,
                        gemPlayBehavior.StartSwappingBack());
                }
            }

            _playContext.SetPlayState(PlayState.GemSwapBack);
        }
        else
        {
            _playContext.SetPlayState(PlayState.WaitingForInput);
        }
    }

    private bool IsUpdateEnabled() =>
      _playContext.PlayState == PlayState.GemMatch;

    private bool TrySearchForMatchingGems()
    {
        var rows = _playContext.GameBoardFields.Rows;

        var columns = _playContext.GameBoardFields.Columns;

        var anyMatchingGemsFound = false;

        for (var rowIndex = 0; rowIndex < rows; rowIndex++)
        {
            var gameBoardFieldsInRow = _playContext
                .GameBoardFields
                .FieldsInRow(rowIndex)
                .ToArray();

            if (TrySearchForMatchingGemsInGroup(gameBoardFieldsInRow))
            {
                anyMatchingGemsFound = true;
            }
        }

        for (var columnIndex = 0; columnIndex < columns; columnIndex++)
        {
            var gameBoardFieldInColumn = _playContext
                .GameBoardFields
                .FieldsInColumn(columnIndex)
                .ToArray();

            if (TrySearchForMatchingGemsInGroup(gameBoardFieldInColumn))
            {
                anyMatchingGemsFound = true;
            }
        }

        return anyMatchingGemsFound;
    }

    private bool TrySearchForMatchingGemsInGroup(
        GameBoardField[] gameBoardFields)
    {
        var matchingGemsCount = 0;

        GemType? previousGemType = null;

        var anyMatchingGemsFound = false;

        for (var gameBoardFieldIndex = 0; gameBoardFieldIndex < gameBoardFields.Length; gameBoardFieldIndex++)
        {
            var gameBoardField = gameBoardFields[gameBoardFieldIndex];

            if (gameBoardField.IsEmpty)
            {
                if (TryMarkMatchingGems(
                    gameBoardFields,
                    matchingGemsCount,
                    gameBoardFieldIndex - 1))
                {
                    anyMatchingGemsFound = true;
                }

                matchingGemsCount = 0;
                previousGemType = null;

                continue;
            }

            var gem = _gemStore.Get(gameBoardField.GemEntity);

            if (previousGemType is null)
            {
                matchingGemsCount = 1;
                previousGemType = gem.GemType;

                continue;
            }

            if (gem.GemType != previousGemType.Value)
            {
                if (TryMarkMatchingGems(
                    gameBoardFields,
                    matchingGemsCount,
                    gameBoardFieldIndex - 1))
                {
                    anyMatchingGemsFound = true;
                }

                matchingGemsCount = 1;
                previousGemType = gem.GemType;

                continue;
            }

            matchingGemsCount++;

            if (gameBoardFieldIndex == gameBoardFields.Length - 1)
            {
                if (TryMarkMatchingGems(
                    gameBoardFields,
                    matchingGemsCount,
                    gameBoardFieldIndex))
                {
                    anyMatchingGemsFound = true;
                }
            }
        }

        return anyMatchingGemsFound;
    }

    private bool TryMarkMatchingGems(
         GameBoardField[] gameBoardFields,
        int matchingGemsCount,
        int lastIndex)
    {
        if (matchingGemsCount >= Constants.Gem.MinimalGemsCountToMatch)
        {
            MarkMatchingGems(
                gameBoardFields,
                matchingGemsCount,
                lastIndex);

            return true;
        }

        return false;
    }

    private void MarkMatchingGems(
        GameBoardField[] gameBoardFields,
        int matchingGemsCount,
        int lastIndex)
    {
        var startIndex = lastIndex - matchingGemsCount + 1;

        for (var index = startIndex; index <= lastIndex; index++)
        {
            var gameBoardField = gameBoardFields[index];

            var gemPlayBehavior = _gemPlayBehaviorStore.Get(
                gameBoardField.GemEntity);

            _gemPlayBehaviorStore.Set(
                gameBoardField.GemEntity,
                gemPlayBehavior.MarkAsMatching());
        }
    }
}
