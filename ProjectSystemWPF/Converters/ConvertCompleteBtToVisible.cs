using ChatServerDTO.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;

namespace ProjectSystemWPF.Converters
{
    public class ConvertCompleteBtToVisible : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mas = value as TaskDTO;
            if (mas == null)
                return Visibility.Collapsed;
            else
                return mas.UStatus == "В процессе" ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        static ConvertCompleteBtToVisible instance = new();
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return instance;
        }
    }

}
