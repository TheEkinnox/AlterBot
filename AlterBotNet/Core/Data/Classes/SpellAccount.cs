﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AlterBotNet.Core.Data.Classes
{
    public class Spell
    {
        internal string SpellName { get; }
        internal SpellType Type { get; }
        internal string SpellFullIncantation { get; }
        internal string SpellEffects { get; }
        internal SpellLevel Level { get; }

        public Spell(string name, SpellType type, string incant, string effects, SpellLevel level)
        {
            this.SpellName = name;
            this.Type = type;
            this.SpellFullIncantation = incant;
            this.SpellEffects = effects;
            this.Level = level;
        }

        public string TextForm()
        {
            string message = $"\n*Nom* : {this.SpellName}\n*Type* : {this.Type}\n*Formule* : {this.SpellFullIncantation}\n*Effet* : {this.SpellEffects}\n*Difficulté* : {this.Level.ToString()}\n";
            return message;
        }

        public override string ToString()
        {
            string message = $"{this.SpellName};{this.Type};{this.SpellFullIncantation};{this.SpellEffects};{this.Level}";
            return message;
        }
    }

    public class SpellAccount
    {
        public string Name { get; }
        public List<Spell> Spells { get; private set; }
        public ulong UserId { get; }

        public SpellAccount(string name, List<Spell> spells, ulong userId = 0)
        {
            this.Name = name;
            this.Spells = spells;
            this.UserId = userId;
        }

        public override string ToString()
        {
            string message = $"{this.Name},";
            foreach (Spell spell in this.Spells)
            {
                message += spell.ToString() + ",";
            }

            message += $"{this.UserId}";
            return message;
        }

        public string TextForm()
        {
            string message = "";
            if (this.Name != "Grimoire public")
                message += $"**Enchantements et sortilèges de {this.Name}:**\n";
            else
                message += "**Grimoire Commun**\n";
            if (this.Spells.Count > 0)
            {
                for (int j = 0; j < this.Spells.Count; j++)
                {
                    if (this.Spells[j]!=null)
                    {
                        message += $"__*Numéro {j}*__ {this.Spells[j].TextForm()}";
                    }
                }
            }
            else message += "\nGrimoire vide";
            return message;
        }

        public bool PossedeSpell(string nomSpell)
        {
            return Global.FindXmlSpell(nomSpell) != null;
        }
    }

    public enum SpellType
    {
        [Description("Sortilège")] Sortilege,
        [Description("Enchantement")] Enchantement
    }

    public enum SpellLevel
    {
        [Description("Simple")] Simple,
        [Description("Basique")] Basique,
        [Description("Avancé")] Avance,
        [Description("Expert")] Expert,
        [Description("Maître")] Maitre
    }
}
