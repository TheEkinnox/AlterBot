using AlterBotNet.Core.Data.Classes;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace AlterBotNet.Core.Commands
{
    public static class PublicGrim
    {
        public static List<Spell> PublicGrimSpells = Global.ChargerDonneesSpell(Global.CheminGrimoirePublic)[0].Spells;

        public static Spell FindSpell(string nomSpell)
        {
            nomSpell = nomSpell.ToLower().Replace("-", " ").Replace("_", " ").Replace("é", "e").Replace("è", "e").Replace("ë", "e").Replace("ä", "a").Replace("ï", "i");
            Spell spell = spell = (from s in PublicGrim.PublicGrimSpells
                     where s.SpellName.ToLower().Replace("é", "e").Replace("è", "e").Replace("ë", "e").Replace("ä", "a").Replace("ï", "i") == nomSpell
                     select s).FirstOrDefault();
            return spell;
        }
    }
}
