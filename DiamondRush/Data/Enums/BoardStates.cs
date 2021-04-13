using System;
using System.Collections.Generic;
using System.Text;

namespace DiamondRush.Data.Enums
{
    public enum BoardStates
    {
        PLAY,
        TRY_SWITCH_GEMS,
        GEMS_SWITCHING,
        GEMS_SWITCHING_BACK,
        GEMS_COLLECTING,
        GEMS_FALLING,
    }
}
