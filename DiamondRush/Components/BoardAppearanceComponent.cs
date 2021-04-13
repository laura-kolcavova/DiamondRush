using DiamondRush.Data.Structs;
using MonoECS.Ecs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiamondRush.Components
{
    public class BoardAppearanceComponent : IEntityComponent
    {
        public BoardTheme Theme { get; set; }

        public int FieldWidth { get; set; }
        public int FieldHeight { get; set; }
        public int FieldSpace { get; set; }
    }
}
