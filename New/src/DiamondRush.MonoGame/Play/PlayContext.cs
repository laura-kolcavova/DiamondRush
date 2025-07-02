using DiamondRush.MonoGame.Play.Components;
using LightECS;

namespace DiamondRush.MonoGame.Play;

internal sealed class PlayContext
{
    public Entity GameBoardEntity { get; }

    public GameBoard GameBoard { get; }

    public GameBoardFields GameBoardFields { get; }

    public PlayState PlayState { get; private set; }

    public PlayContext(
        Entity gameBoardEntity,
        GameBoard gameBoard,
        GameBoardFields gameBoardFields,
        PlayState initialPlayState)
    {
        GameBoardEntity = gameBoardEntity;

        GameBoard = gameBoard;

        GameBoardFields = gameBoardFields;

        PlayState = initialPlayState;
    }

    public void SetPlayState(
        PlayState playState)
    {
        PlayState = playState;
    }

    public static PlayContext CreateDefault(
        Entity gameBoardEntity,
        GameBoard gameBoard)
    {
        var gameBoardFields = new GameBoardFields(gameBoard);

        return new PlayContext(
            gameBoardEntity,
            gameBoard,
            gameBoardFields,
            PlayState.SpawningNewGems);
    }
}
