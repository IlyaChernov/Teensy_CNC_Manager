namespace TeensyCncManager.ViewModelUtils
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System;
    using System.Collections.Generic;

    public class PointsToPathConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var points = (Point[])value;
            if (points != null && points.Length > 0)
            {
                var offset = new Point((points.Min(x => x.X)), (points.Min(y => y.Y)));

                offset.X = offset.X > 0 ? 0 : Math.Abs(offset.X);
                offset.Y = offset.Y > 0 ? 0 : Math.Abs(offset.Y);

                var start = new Point(points[0].X + offset.X, points[0].Y + offset.Y);
                var segments = new List<LineSegment>();
                for (int i = 1; i < points.Length; i++)
                {
                    segments.Add(new LineSegment(new Point(points[i].X + offset.X, points[i].Y + offset.Y), true));
                }
                var figure = new PathFigure(start, segments, false);
                var geometry = new PathGeometry();
                geometry.Figures.Add(figure);

                return geometry;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
