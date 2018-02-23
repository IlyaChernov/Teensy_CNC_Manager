namespace TeensyCncManager.ViewModelUtils
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class DistanceConverter : IValueConverter
   {       
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)(long)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (long)(double)value;
        }
    }
}
