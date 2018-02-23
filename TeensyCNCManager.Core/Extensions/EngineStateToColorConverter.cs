using System;

namespace TeensyCNCManager.Core.Extensions
{
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public class EngineStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = (EngineState)value;
            switch (date)
            {
                case EngineState.Running:
                    
                    return Brushes.DarkSeaGreen;
                case EngineState.Paused:
                    return Brushes.DarkOrange;
                case EngineState.EmergencyStopped:
                    return Brushes.OrangeRed;
                default:
                    return Brushes.DarkSeaGreen;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
