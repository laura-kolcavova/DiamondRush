using DiamondRush.MonoGame.Core;
using DiamondRush.MonoGame.Core.Systems;
using DiamondRush.MonoGame.Play.Components;
using DiamondRush.MonoGame.Play.Content.Abstractions;
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

        var emptyGameBoardFields = FindEmptyGameBoardFields(
            _playContext.GameBoardFields);

        if (!emptyGameBoardFields.Any())
        {
            _playContext.SetPlayState(PlayState.WaitingForInput);

            return;
        }

        foreach (var gameBoardField in emptyGameBoardFields)
        {
            CreateGemForGameBoardField(gameBoardField);
        }

        _playContext.SetPlayState(PlayState.FallingGems);
    }

    private bool IsUpdateEnabled() =>
        _playContext.PlayState == PlayState.SpawningNewGems;

    private IEnumerable<GameBoardField> FindEmptyGameBoardFields(
        GameBoardFields gameBoardFields)
    {
        return _playContext
            .GameBoardFields
            .AsEnumerable()
            .Where(gameBoardField => gameBoardField.IsEmpty);
    }

    private Entity CreateGemForGameBoardField(
        GameBoardField gameBoardField)
    {
        var gameBoardFieldPosition = GetGameBoardFieldPosition(
          gameBoardField);

        var gameBoardRectTransform = _rectTransformStore.Get(
           _playContext.GameBoardEntity);

        var gemPosition = new Vector2(
            gameBoardFieldPosition.X,
            gameBoardFieldPosition.Y - gameBoardRectTransform.Height);

        var gemType = GetRandomGemType();

        var gemEntity = _gemEntityFactory.Create(
            _entityContext,
            gemType,
            gemPosition);

        var gemPlayBehavior = new GemPlayBehavior();

        _gemPlayBehaviorStore.Set(
            gemEntity,
            gemPlayBehavior.StartFallingToGameBoardField(
                gameBoardField,
                gameBoardFieldPosition));

        return gemEntity;
    }

    private Vector2 GetGameBoardFieldPosition(
        GameBoardField gameBoardField)
    {
        var gameBoardRectTransform = _rectTransformStore.Get(
            _playContext.GameBoardEntity);

        var gameBoardFieldPositionX = gameBoardField.ColumnIndex
            * (Constants.GameBoardFieldSize + Constants.GameBoardSpacingWidth);

        var gameBoardFieldPositionY = gameBoardField.RowIndex
            * (Constants.GameBoardFieldSize + Constants.GameBoardSpacingWidth);

        return new Vector2(
            gameBoardRectTransform.Position.X + gameBoardFieldPositionX,
            gameBoardRectTransform.Position.Y + gameBoardFieldPositionY);
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
