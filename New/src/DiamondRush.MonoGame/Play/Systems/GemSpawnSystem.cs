using DiamondRush.MonoGame.Core;
using DiamondRush.MonoGame.Core.Systems;
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

    private readonly Random _random;

    public GemSpawnSystem(
        IEntityContext entityContext,
        IPlaySceneContentProvider playSceneContentProvider,
        PlayContext playContext)
    {
        _entityContext = entityContext;

        _playContext = playContext;

        _gemEntityFactory = new GemEntityFactory(playSceneContentProvider);

        _rectTransformStore = _entityContext.UseStore<RectTransform>();

        _random = new Random();
    }

    public void Update(GameTime gameTime)
    {
        if (!IsUpdateEnabled())
        {
            return;
        }

        var emptyGameBoardFields = FindEmptyGameBoardFields(
            _playContext.GameBoardFields);

        if (emptyGameBoardFields.Count == 0)
        {
            _playContext.SetPlayState(PlayState.WaitingForInput);

            return;
        }

        foreach (var gameBoardField in emptyGameBoardFields)
        {
            CreateGemForGameBoardField(gameBoardField);
        }

        _playContext.SetPlayState(PlayState.WaitingForInput);
    }

    private bool IsUpdateEnabled() =>
        _playContext.PlayState == PlayState.SpawningNewGems;

    private IReadOnlyCollection<GameBoardField> FindEmptyGameBoardFields(
        GameBoardFields gameBoardFields)
    {
        return _playContext
            .GameBoardFields
            .AsEnumerable()
            .Where(gameBoardField => gameBoardField.IsEmpty)
            .ToList();
    }

    private Entity CreateGemForGameBoardField(
        GameBoardField gameBoardField)
    {
        var position = GetGameBoardFieldPosition(
            gameBoardField);

        var gemType = GetRandomGemType();

        return _gemEntityFactory.Create(
            _entityContext,
            gemType,
            position);
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
