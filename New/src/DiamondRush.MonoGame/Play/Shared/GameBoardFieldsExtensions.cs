namespace DiamondRush.MonoGame.Play.Shared;

internal static class GameBoardFieldsExtensions
{
    public static IEnumerable<GameBoardField> EmptyFields(
        this GameBoardFields gameBoardFields)
    {
        return gameBoardFields
            .AsEnumerable()
            .Where(gameBoardField => gameBoardField.IsEmpty);
    }
}
