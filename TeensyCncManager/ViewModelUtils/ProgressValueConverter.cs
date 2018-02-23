namespace TeensyCncManager.ViewModelUtils
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class ProgressValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {           
            return $"{(long)values[1]} / {(long)values[0]}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
