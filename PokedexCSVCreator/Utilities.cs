using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokedex.PokedexCSVCreator
{
    public static class Utilities
    {
        public static bool IsKommooLine(string name) => name.Contains("jangmo-o") || name.Contains("hakamo-o") || name.Contains("kommo-o");
        public static bool IsMrMimeLine(string name) => name.Contains("mr-mime") || name.Contains("mime-jr") || name.Contains("mr-rime");
    }
}
