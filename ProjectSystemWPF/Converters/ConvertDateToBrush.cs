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
    public class ConvertDateToBrush : MarkupExtension, IMultiValueConverter
    {
        
        static ConvertDateToBrush instance = new();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is DateTime startDate && values[1] is DateTime deadline)
            {
                // Получаем текущую дату
                DateTime currentDate = DateTime.Now;

                // Проверяем, что дедлайн не раньше даты начала
                //if (deadline < startDate)
                //{
                //    throw new ArgumentException("Deadline must be after start date.");
                //}

                // Вычисляем общее время от начала до дедлайна
                TimeSpan totalDuration = deadline - startDate;
                // Вычисляем время, прошедшее с начала проекта до текущей даты
                TimeSpan elapsedDuration = currentDate - startDate;

                // Вычисляем процент завершенности
                double completionPercentage = (elapsedDuration.TotalMilliseconds / totalDuration.TotalMilliseconds) * 100;

                // Определяем цвет на основе процентного соотношения
                if (completionPercentage < 50)
                {
                    return Brushes.LightGreen; // Менее 50%
                }
                else if (completionPercentage >= 50 && completionPercentage < 70)
                {
                    return Brushes.Yellow; // От 50% до 70%
                }
                else if (completionPercentage >= 70 && completionPercentage < 90)
                {
                    return Brushes.Orange; // От 70% до 90%
                }
                else if (completionPercentage >= 90 && completionPercentage <= 100)
                {
                    return Brushes.Red; // От 90% до 100%
                }
                else
                    return Brushes.LightGray;
            }

            return Brushes.Transparent; // Возвращаем прозрачный цвет по умолчанию
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return instance;
        }
    }
}
