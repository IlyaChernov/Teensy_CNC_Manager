namespace TeensyCNCManager.Core.GCode
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Media.Media3D;

    using TeensyCNCManager.Core.Interpolation;

    [Code(CodeName = "G02")]
    public class G02 : IGcode, IRadialInterpolation, IExpandable
    {
        public void ClearParams()
        {
            foreach (var propertyInfo in GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(CodeParameter), true).All(x => !((CodeParameter)x).Persistent)))
            {
                propertyInfo.SetValue(this, null);
            }
        }

        public void WipeOutParams()
        {
            foreach (var propertyInfo in GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(CodeParameter), true).Any()))
            {
                propertyInfo.SetValue(this, null);
            }
        }

        public void ApplyParam(string param)
        {
            foreach (var propertyInfo in GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(CodeParameter), true).Any()))
            {
                var req = propertyInfo.GetCustomAttributes(true);
                var separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                foreach (CodeParameter o in req)
                {
                    if (param.StartsWith(o.ParamName))
                        propertyInfo.SetValue(this, double.Parse(param.Substring(1).Replace(".", separator).Replace(",", separator)));
                }
            }
        }

        public RadialInterpolationDirection Direction = RadialInterpolationDirection.ClockWise;

        public double? XStart { get; set; }

        public double? YStart { get; set; }

        public double? ZStart { get; set; }

        [CodeParameter(ParamName = "X")]
        [CodeParameter(ParamName = "x")]
        public double? XDestination { get; set; }

        [CodeParameter(ParamName = "Y")]
        [CodeParameter(ParamName = "y")]
        public double? YDestination { get; set; }

        [CodeParameter(ParamName = "Z")]
        [CodeParameter(ParamName = "z")]
        public double? ZDestination { get; set; }

        [CodeParameter(ParamName = "I")]
        [CodeParameter(ParamName = "i")]
        public double? IDistance { get; set; }

        [CodeParameter(ParamName = "J")]
        [CodeParameter(ParamName = "j")]
        public double? JDistance { get; set; }

        [CodeParameter(ParamName = "K")]
        [CodeParameter(ParamName = "k")]
        public double? KDistance { get; set; }

        [CodeParameter(ParamName = "R")]
        [CodeParameter(ParamName = "r")]
        public double? RDistance { get; set; }

        [CodeParameter(ParamName = "F", Persistent = true)]
        [CodeParameter(ParamName = "f")]
        public double? FSpeed { get; set; }

        public IEnumerable<IGcode> Expand(IGcode prevCommand, double defaultSpeed, double stepSize, double angle)
        {
            if (prevCommand == null) throw new ArgumentNullException("prevCommand");

            var startPoint = new Point3D(prevCommand.XDestination ?? 0, prevCommand.YDestination ?? 0, prevCommand.ZDestination ?? 0);

            var endPoint = new Point3D(XDestination ?? prevCommand.XDestination ?? 0, YDestination ?? prevCommand.YDestination ?? 0, ZDestination ?? prevCommand.ZDestination ?? 0);

            RadialInterpolation ri;

            if (RDistance.HasValue)
            {
                ri = new RadialInterpolation(startPoint, endPoint, RDistance.Value, Direction);
            }
            else
            {
                var center = new Point3D(
                    startPoint.X + IDistance ?? 0,
                    startPoint.Y + JDistance ?? 0,
                    startPoint.Z + KDistance ?? 0);

                ri = new RadialInterpolation(startPoint, center, endPoint, Direction);
            }

            var pts = ri.GetArcPoints(stepSize, angle);
            var prevPoint = new Point3D(
                prevCommand.XDestination ?? 0,
                prevCommand.YDestination ?? 0,
                prevCommand.ZDestination ?? 0);
            foreach (var point3D in pts)
            {
                var spd = FSpeed;
                if (prevCommand is IMovementSpeed)
                {
                    spd = FSpeed ?? (prevCommand as IMovementSpeed).FSpeed ?? defaultSpeed;
                }

                yield return new G01 { FSpeed = spd, XStart = prevPoint.X, YStart = prevPoint.Y, ZStart = prevPoint.Z, XDestination = point3D.X, YDestination = point3D.Y, ZDestination = point3D.Z };

                prevPoint = point3D;
            }
        }
    }
}