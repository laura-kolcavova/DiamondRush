using DiamondRush.Data;
using DiamondRush.Data.Classes;
using DiamondRush.Data.Enums;
using MonoECS.Ecs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiamondRush.Components
{
    public class GemComponent : IEntityComponent
    { 
        public GemTypes GemType { get; set; }

        public GemField GemField { get; private set; }

        public bool IsAttachedToGemField => GemField != GemField.NONE;

        public void AttachToGemField(GemField gemField) => GemField = gemField;

        public void DettachFromGemField() => GemField = GemField.NONE;
    }
}
