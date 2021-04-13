using System;
using System.Collections.Generic;
using System.Text;

namespace DiamondRush.Data.Classes
{
    public class GemField
    {
        public static readonly GemField NONE = null;

        public GemField(int gameBoardId, int row, int col)
        {
            GameBoardId = gameBoardId;
            Row = row;
            Col = col;

            GemId = -1;
        }

        public int GameBoardId { get; }
        public int Row { get; }
        public int Col { get; }

        public int GemId { get; private set; }

        public void AttachGem(int gem)
        {
            GemId = gem;
        }

        public void DetachGem()
        {
            GemId = -1;
        }

        public bool IsEmpty => GemId == -1;
    }
}
