using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play;

internal static class Constants
{
    public static class GameBoard
    {
        public const int RowCount = 8;

        public const int ColumnCount = 8;

        public const int FieldSize = 64;

        public readonly static Color FieldColor = new Color(Color.Black, 0.8f);

        public readonly static Color BorderColor = Color.SaddleBrown;

        public readonly static Color SpacingColor = new Color(66, 66, 66);

        public const int SpacingWidth = 1;

        public const int BorderWidth = 3;
    }

    public static class Gem
    {
        public const int MinimalGemsCountToMatch = 3;

        public const int Width = 64;

        public const int Height = 64;

        public const float FallSpeed = 500f;

        public const float SwapSpeed = 250f;

        public const float CollectAnimationDurationInSeconds = 0.5f;
    }

    public static class LayerDepth
    {
        public const float Background = 0f;

        public const float GameBoardField = 0.1f;

        public const float GameBoardBorder = 0.11f;

        public const float GameBoardSpacing = 0.11f;

        public const float Gem = 0.2f;

        public const float SwappingMasterGem = 0.3f;
    }
}
