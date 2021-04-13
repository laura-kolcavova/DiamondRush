using DiamondRush.Data.Structs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiamondRush.Data
{
    public static class BoardThemes
    {
        public static readonly BoardTheme DEFAULT = new BoardTheme()
        {
            FieldColor = new Color(Color.Black, 0.7f),
            FieldSpaceColor = new Color(0, 0, 0, 0.5f),
            BorderColor = Color.SaddleBrown,

            BorderWidth = 3,
        };
    }
}
