using Pokedex.PkdxDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Pokedex.PokedexApp.Utilities
{
    public class MyTemplateSelector : DataTemplateSelector
    {
        private bool hasMultiple;
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is Pokemon)
            {
                var pkmn = item as Pokemon;

                if (hasMultiple)
                {
                    hasMultiple = false;
                    return element.FindResource("template3") as DataTemplate;
                }

                hasMultiple = pkmn.HasMultipleEvolutions;
                
                if (string.IsNullOrEmpty(pkmn.EvolveMethodString) || pkmn.EvolveMethodString == "N")
                    return element.FindResource("template2") as DataTemplate;
                else if (!string.IsNullOrEmpty(pkmn.EvolveMethodString))
                    return element.FindResource("template1") as DataTemplate;
            }

            return null;
        }
    }
}
