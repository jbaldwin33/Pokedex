using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pokedex.PokedexLib.Enums;

namespace Pokedex.PkdxDatabase.Models
{
    public class BaseStat
    {
        public StatEnum Stat { get; set; }
        public int Value { get; set; }

        public BaseStat(StatEnum stat, int val)
        {
            Stat = stat;
            Value = val;
        }
    }
}
