namespace TeensyCncManager.ViewModelUtils
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Data;

    [ValueConversion(typeof(List<string>), typeof(string))]
    public class ListToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a String");

            return String.Join(Environment.NewLine, ((List<string>)value).ToArray());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var srtings = ((string)value).Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            return new List<string>(srtings);
        }
    }
}