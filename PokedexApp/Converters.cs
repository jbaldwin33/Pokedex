using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Pokedex.PokedexApp
{
    public class MultiplierConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => $"{value}x";

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => value;
    }

    public class EnumValueToDecriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            var type = value.GetType();
            if (!type.IsEnum)
            {
                return null;
            }
            var field = type.GetField(value.ToString());
            var attr = field.GetCustomAttributes(typeof(DescriptionAttribute), true)
                            .Cast<DescriptionAttribute>()
                            .FirstOrDefault();
            if (attr != null)
            {
                return attr.Description;
            }
            else
            {
                return field.Name;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}