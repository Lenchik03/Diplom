using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace ProjectSystemWPF.Converters
{
    public class ConvertStatusToBrush : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = (string)value;
            if (status == "Выдана")
                return Brushes.Yellow;

            else if (status == "В процессе")
                return Brushes.Green;

            else if (status == "Завершена")
                return Brushes.LightBlue;

            else
                return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        static ConvertStatusToBrush instance = new();
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return instance;
        }
    }
}
