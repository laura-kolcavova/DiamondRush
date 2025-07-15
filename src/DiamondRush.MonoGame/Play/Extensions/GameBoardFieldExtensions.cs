using DiamondRush.MonoGame.Core;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play.Extensions;

internal static class GameBoardFieldExtensions
{
    public static Vector2 ComputePosition(
        this GameBoardField gameBoardField,
        RectTransform gameBoardRectTransform)
    {
        var gameBoardFieldPositionX = gameBoardField.ColumnIndex
            * (Constants.GameBoard.FieldSize + Constants.GameBoard.SpacingWidth);

        var gameBoardFieldPositionY = gameBoardField.RowIndex
            * (Constants.GameBoard.FieldSize + Constants.GameBoard.SpacingWidth);

        return new Vector2(
            gameBoardRectTransform.Position.X + gameBoardFieldPositionX,
            gameBoardRectTransform.Position.Y + gameBoardFieldPositionY);
    }
}
