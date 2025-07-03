using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play;

internal static class Constants
{
    public const int GameBoardRows = 8;

    public const int GameBoardColumns = 8;

    public const int GameBoardFieldSize = 64;

    public readonly static Color GameBoardColor = new Color(Color.Black, 0.8f);

    public readonly static Color GameBoardBorderColor = Color.SaddleBrown;

    public readonly static Color GameBoardSpacingColor = new Color(66, 66, 66);

    public const int GameBoardSpacingWidth = 1;

    public const int GameBoardBorderWidth = 3;

    public const int GemWidth = 64;

    public const int GemHeight = 64;
}
