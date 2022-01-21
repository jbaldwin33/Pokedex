using System;
using System.Globalization;

namespace Pokedex.PokedexCSVCreator
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input)
        {
            var textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(input);//input[0].ToString().ToUpper() + input.Substring(1);
        }
    }
}
