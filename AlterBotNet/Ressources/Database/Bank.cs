using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace AlterBotNet.Ressources.Database
{
    public class Bank
    {
        [Key]
        public ulong UserId { get; set; }
        public string Name { get; set; }
        public int Amout { get; set; }
    }
}
