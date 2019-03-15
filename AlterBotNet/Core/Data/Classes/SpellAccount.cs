using System;
using System.Collections.Generic;
using System.Text;

namespace AlterBotNet.Core.Data.Classes
{
    class SpellAccount
    {
        SpellType Type { get; set; }
        string SpellName { get; set; }
        string SpellFullIncantation { get; set; }
        string SpellEffects { get; set; }
        SpellLevel Level { get; set; } 
    }
}
