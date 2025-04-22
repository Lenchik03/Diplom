using ProjectSystemAPI.DB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ProjectSystemWPF.Converters
{
    internal class ConvertUserToAlign : MarkupExtension,  IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return TextAlignment.Left;
            if ((int)value == ActiveUser.GetInstance().User.Id)
                return TextAlignment.Right;
            return TextAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        static ConvertUserToAlign instance = new();
        // это требуется для MarkupExtension - типа метод получения экземпляра
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return instance;
        }
    }
}
