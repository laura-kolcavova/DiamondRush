using System;
using System.Collections.Generic;
using System.Text;

namespace DiamondRush.Data
{
    public static class Options
    {
        public struct GameApp
        {
            public const int Default_Width = 1920;
            public const int Default_Height = 1080;
        }

        public struct GameBoard
        {
            public const int Field_Width = 64;
            public const int Field_Height = 64;     
            public const int Field_Space = 1;
            public const int MinCountForGemGroup = 3;
        }

        public struct Gem
        {
            public const float Switch_Speed = 250f;
            public const float Fall_Speed = 500f;
        }

        
    }
}
