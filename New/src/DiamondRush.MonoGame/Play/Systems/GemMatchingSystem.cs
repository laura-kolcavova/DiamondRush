﻿using DiamondRush.MonoGame.Core.Systems;
using DiamondRush.MonoGame.Play.Components;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play.Systems;

internal sealed class GemMatchingSystem :
    IUpdateSystem
{
    private readonly PlayContext _playContext;

    private readonly IComponentStore<Gem> _gemStore;

    private readonly IComponentStore<GemPlayBehavior> _gemPlayBehaviorStore;


    public GemMatchingSystem(
        IEntityContext entityContext,
        PlayContext playContext)
    {
        _playContext = playContext;

        _gemStore = entityContext.UseStore<Gem>();
        _gemPlayBehaviorStore = entityContext.UseStore<GemPlayBehavior>();
    }

    public void Update(GameTime gameTime)
    {
        if (!IsUpdateEnabled())
        {
            return;
        }

        if (SearchForMatchingGems())
        {
            _playContext.SetPlayState(PlayState.CollectingGems);
        }
        else
        {
            _playContext.SetPlayState(PlayState.WaitingForInput);
        }
    }

    private bool SearchForMatchingGems()
    {
        var rows = _playContext.GameBoardFields.Rows;

        var columns = _playContext.GameBoardFields.Columns;

        var anyMatchingGemsFound = false;

        for (var rowIndex = 0; rowIndex < rows; rowIndex++)
        {
            var gameBoardFieldsInRow = _playContext
                .GameBoardFields
                .GetFieldsInRow(rowIndex)
                .ToArray();

            if (SearchForMatchingGemsInGroup(gameBoardFieldsInRow))
            {
                anyMatchingGemsFound = true;
            }
        }

        for (var columnIndex = 0; columnIndex < columns; columnIndex++)
        {
            var gameBoardFieldInColumn = _playContext
                .GameBoardFields
                .GetFieldsInColumn(columnIndex)
                .ToArray();

            if (SearchForMatchingGemsInGroup(gameBoardFieldInColumn))
            {
                anyMatchingGemsFound = true;
            }
        }

        return anyMatchingGemsFound;
    }

    private bool SearchForMatchingGemsInGroup(
        GameBoardField[] gameBoardFields)
    {
        var matchingGemsCount = 0;

        GemType? previousGemType = null;

        var anyMatchingGemsFound = false;

        var lastIndex = -1;

        foreach (var gameBoardField in gameBoardFields)
        {
            lastIndex++;

            if (gameBoardField.IsEmpty)
            {
                if (TryMarkMatchingGems(
                    gameBoardFields,
                    matchingGemsCount,
                    lastIndex))
                {
                    anyMatchingGemsFound = true;
                }

                matchingGemsCount = 0;
                previousGemType = null;

                continue;
            }

            var gem = _gemStore.Get(gameBoardField.GemEntity);

            if (previousGemType is not null &&
                gem.GemType == previousGemType.Value)
            {
                matchingGemsCount++;

                continue;
            }

            if (TryMarkMatchingGems(
                gameBoardFields,
                matchingGemsCount,
                lastIndex))
            {
                anyMatchingGemsFound = true;
            }

            matchingGemsCount = 1;
            previousGemType = gem.GemType;
        }

        return anyMatchingGemsFound;
    }

    private bool TryMarkMatchingGems(
         GameBoardField[] gameBoardFields,
        int matchingGemsCount,
        int lastIndex)
    {
        if (matchingGemsCount >= Constants.MinimalGemsCountToMatch)
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
                gemPlayBehavior.SetIsMatching(true));
        }
    }


    private bool IsUpdateEnabled() =>
       _playContext.PlayState == PlayState.MatchingGems;
}
