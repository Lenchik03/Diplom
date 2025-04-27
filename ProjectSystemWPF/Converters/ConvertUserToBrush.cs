using ProjectSystemAPI.DB;
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
    internal class ConvertUserToBrush : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Brushes.AliceBlue;
            if ((int)value == ActiveUser.GetInstance().User.Id)
                return Brushes.LightGreen;
            if ((int)value != ActiveUser.GetInstance().User.Id)
                return Brushes.AliceBlue;
            return Brushes.AliceBlue;
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
