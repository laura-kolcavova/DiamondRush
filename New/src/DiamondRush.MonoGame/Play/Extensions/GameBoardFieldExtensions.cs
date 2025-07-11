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
            * (Constants.GameBoardFieldSize + Constants.GameBoardSpacingWidth);

        var gameBoardFieldPositionY = gameBoardField.RowIndex
            * (Constants.GameBoardFieldSize + Constants.GameBoardSpacingWidth);

        return new Vector2(
            gameBoardRectTransform.Position.X + gameBoardFieldPositionX,
            gameBoardRectTransform.Position.Y + gameBoardFieldPositionY);
    }
}
