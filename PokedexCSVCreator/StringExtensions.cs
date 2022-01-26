using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Pokedex.PokedexCSVCreator
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input)
        {
            var textInfo = new CultureInfo("en-US", false).TextInfo;
            if (input.Contains("jangmo-o") || input.Contains("hakamo-o") || input.Contains("kommo-o"))
                return textInfo.ToTitleCase(input).ToIrishCase();
            if (input.Contains("mime-jr"))
                return textInfo.ToTitleCase(input.Replace('-', ' ').Insert(input.Length, "."));
            if (input.Contains("mr-mime") || input.Contains("mr-rime"))
                return textInfo.ToTitleCase(input.Replace("-", ". "));
            return textInfo.ToTitleCase(input);
        }

        public static string ToIrishCase(this string s)
        {
            var titlecase = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLowerInvariant());
            return Regex.Replace(titlecase, "-(?:.)", m => m.Value.ToLowerInvariant());
        }

        public static IEnumerable<string> SplitBy(string input, char separator, int n)
        {
            int lastindex = 0;
            int curr = 0;

            while (curr < input.Length)
            {
                int count = 0;
                while (curr < input.Length && count < n)
                {
                    if (input[curr++] == separator) count++;
                }
                yield return input.Substring(lastindex, curr - lastindex - (curr < input.Length ? 1 : 0));
                lastindex = curr;
            }
        }
    }
}
