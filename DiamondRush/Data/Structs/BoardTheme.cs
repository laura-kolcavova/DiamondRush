using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiamondRush.Data.Structs
{
    public struct BoardTheme
    {
        public Color FieldColor { get; set; }
        public Color FieldSpaceColor { get; set; }
        public Color BorderColor { get; set; }

        public int BorderWidth { get; set; }
    }
}
