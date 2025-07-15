using DiamondRush.MonoGame.Core;
using DiamondRush.MonoGame.Play.Components;
using DiamondRush.MonoGame.Play.Extensions;
using LightECS;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play;

internal sealed class PlayContext
{
    public Entity GameBoardEntity { get; }

    public GameBoard GameBoard { get; }

    public GameBoardFields GameBoardFields { get; }

    public PlayState PlayState { get; private set; }

    public PlayState PreviousPlayState { get; private set; }

    private Dictionary<GameBoardField, Vector2> _gameBoardFieldPositions;

    private readonly Dictionary<int, List<Entity>> _spawnedGemEntitiesByColumnIndex;

    private PlayContext(
        Entity gameBoardEntity,
        GameBoard gameBoard,
        GameBoardFields gameBoardFields)
    {
        GameBoardEntity = gameBoardEntity;

        GameBoard = gameBoard;

        GameBoardFields = gameBoardFields;

        PlayState = PlayState.WaitingForInput;

        _gameBoardFieldPositions = [];

        _spawnedGemEntitiesByColumnIndex = [];
    }

    public void SetPlayState(
        PlayState playState)
    {
        PreviousPlayState = PlayState;
        PlayState = playState;
    }

    public void ComputeGameBoardFieldPositions(
        RectTransform gameBoardRectTransform)
    {
        _gameBoardFieldPositions.Clear();

        for (var rowIndex = 0; rowIndex < GameBoardFields.Rows; rowIndex++)
        {
            for (var columnIndex = 0; columnIndex < GameBoardFields.Columns; columnIndex++)
            {
                var gameBoardField = GameBoardFields.GetField(
                    rowIndex,
                    columnIndex);

                var gameBoardFieldPosition = gameBoardField.ComputePosition(gameBoardRectTransform);

                _gameBoardFieldPositions.Add(gameBoardField, gameBoardFieldPosition);
            }
        }
    }

    public Vector2 GetGameBoardFieldPosition(
        GameBoardField gameBoardField)
    {
        return _gameBoardFieldPositions[gameBoardField];
    }

    public void AddSpawnedGemEntity(
        int columnIndex,
        Entity gemEntity,
        out int stackCount)
    {

        if (_spawnedGemEntitiesByColumnIndex.TryGetValue(
            columnIndex,
            out var stack))
        {
            stack.Add(gemEntity);

            stackCount = stack.Count;
        }
        else
        {
            stack = [gemEntity];

            stackCount = 1;

            _spawnedGemEntitiesByColumnIndex.Add(columnIndex, stack);
        }
    }

    public bool TryGetSpawnedGemEntities(
        int columnIndex,
        out IReadOnlyCollection<Entity> spawnedGemEntityStack)
    {
        if (_spawnedGemEntitiesByColumnIndex.TryGetValue(
            columnIndex,
            out var value))
        {
            spawnedGemEntityStack = value.AsReadOnly();
            return true;
        }
        else
        {
            spawnedGemEntityStack = [];
            return false;
        }
    }

    public void ClearSpawnedGemEntities()
    {
        _spawnedGemEntitiesByColumnIndex.Clear();
    }

    public static PlayContext CreateDefault(
       Entity gameBoardEntity,
       GameBoard gameBoard)
    {
        var gameBoardFields = new GameBoardFields(gameBoard);

        return new PlayContext(
            gameBoardEntity,
            gameBoard,
            gameBoardFields);
    }
}
