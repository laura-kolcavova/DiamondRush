﻿using DiamondRush.MonoGame.Core;
using DiamondRush.MonoGame.Core.Systems.Abstractions;
using DiamondRush.MonoGame.Play.Components;
using DiamondRush.MonoGame.Play.Content.Abstractions;
using DiamondRush.MonoGame.Play.Extensions;
using DiamondRush.MonoGame.Play.Factories;
using LightECS;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play.Systems;

internal sealed class GemSpawnSystem :
    IUpdateSystem
{
    private readonly IEntityContext _entityContext;

    private readonly PlayContext _playContext;

    private readonly GemEntityFactory _gemEntityFactory;

    private readonly IComponentStore<RectTransform> _rectTransformStore;

    private readonly IComponentStore<GemPlayBehavior> _gemPlayBehaviorStore;

    public GemSpawnSystem(
        IEntityContext entityContext,
        IPlaySceneContentProvider playSceneContentProvider,
        PlayContext playContext)
    {
        _entityContext = entityContext;

        _playContext = playContext;

        _gemEntityFactory = new GemEntityFactory(playSceneContentProvider);

        _rectTransformStore = _entityContext.UseStore<RectTransform>();

        _gemPlayBehaviorStore = _entityContext.UseStore<GemPlayBehavior>();
    }

    public void Update(GameTime gameTime)
    {
        if (!IsUpdateEnabled())
        {
            return;
        }

        if (TrySpawnNewGems())
        {
            _playContext.SetPlayState(PlayState.GemFallInitiation);
        }
        else
        {
            _playContext.SetPlayState(PlayState.WaitingForInput);
        }
    }

    private bool IsUpdateEnabled() =>
        _playContext.PlayState == PlayState.GemSpawn;

    private bool TrySpawnNewGems()
    {
        var emptyGameBoardFields = _playContext
            .GameBoardFields
            .EmptyFields()
            .ToList();

        if (emptyGameBoardFields.Count == 0)
        {
            return false;
        }

        foreach (var gameBoardField in emptyGameBoardFields)
        {
            CreateGemForGameBoardField(gameBoardField);
        }

        return true;
    }

    private Entity CreateGemForGameBoardField(
        GameBoardField gameBoardField)
    {
        var gemType = GetRandomGemType();

        var gemEntity = _gemEntityFactory.Create(
            _entityContext,
            gemType);

        _playContext.AddSpawnedGemEntity(
            gameBoardField.ColumnIndex,
            gemEntity,
            out var stackCount);

        var gameBoardEntity = _playContext.GameBoardEntity;

        var gameBoardRectTransform = _rectTransformStore.Get(
            gameBoardEntity);

        var gameBoardFieldPosition = _playContext.GetGameBoardFieldPosition(gameBoardField);

        var gemPosition = new Vector2(
            gameBoardFieldPosition.X,
            gameBoardRectTransform.Top - Constants.GameBoard.FieldSize * stackCount);

        var gemRectTransform = _rectTransformStore.Get(gemEntity);

        _rectTransformStore.Set(
            gemEntity,
            gemRectTransform.UpdatePosition(gemPosition));

        var gemPlayBehavior = new GemPlayBehavior();

        _gemPlayBehaviorStore.Set(
            gemEntity,
            gemPlayBehavior.SetVisibility(false));

        return gemEntity;
    }

    private static GemType GetRandomGemType()
    {
        var gemTypes = Enum.GetValues<GemType>();

        var randomIndex = Random
            .Shared
            .Next(0, gemTypes.Length);

        return gemTypes[randomIndex];
    }
}
