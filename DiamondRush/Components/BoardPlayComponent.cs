using DiamondRush.Data.Enums;
using MonoECS.Ecs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiamondRush.Components
{
    public class BoardPlayComponent : IEntityComponent
    {
        public BoardPlayComponent()
        {
            ActiveGemId = -1;
            PassiveGemId = -1;
        }

        public BoardStates State { get; set; }

        public int ActiveGemId { get; set; }

        public int PassiveGemId { get; set; }

        public int CollectedGemsCnt { get; set; }

        public int[] CollectingGems { get; set; }

        public int[] RefillingGems { get; set; }

        public int[] FallingGems { get; set; }       
    }
}
