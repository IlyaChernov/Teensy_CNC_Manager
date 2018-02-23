namespace TeensyCNCManager.Core.Interpolation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media.Media3D;

    using TeensyCNCManager.Core.Extensions;

    public class RadialInterpolation
    {
        public double ArcAngleDeg, ArcAngleRad, ArcHorde, ArcRadius;

        internal Point3D StartPoint, CenterPoint, EndPoint;

        internal Point StartFlatPoint, CenterFlatPoint, EndFlatPoint;
        internal double Alpha, Beta;
        internal RadialInterpolationDirection Direction;

        public RadialInterpolation(Point3D startPoint, Point3D endPoint, double radius, RadialInterpolationDirection direction)
        {
            StartPoint = startPoint;
            StartFlatPoint = new Point(startPoint.X, startPoint.Y);
            EndPoint = endPoint;
            EndFlatPoint = new Point(endPoint.X, endPoint.Y);
            ArcRadius = radius;
            Direction = direction;

            ArcHorde = (StartFlatPoint - EndFlatPoint).Length;
            var sidesDoubled = radius * 2;

            var startEndVector = EndFlatPoint - StartFlatPoint;
            var middlePoint = StartFlatPoint + startEndVector * 0.5;
            var heigth = Math.Sqrt(sidesDoubled * sidesDoubled - startEndVector.Length * startEndVector.Length) / 2;

            var middleToVertexVector = startEndVector.Rotate(90) * (heigth / startEndVector.Length);

            if (Direction == RadialInterpolationDirection.CounterClockWise)
            {
                CenterFlatPoint = middlePoint + middleToVertexVector;
                CenterPoint = new Point3D(CenterFlatPoint.X, CenterFlatPoint.Y, EndPoint.Z - StartPoint.Z);
            }
            else
            {
                CenterFlatPoint = middlePoint + (-middleToVertexVector);
                CenterPoint = new Point3D(CenterFlatPoint.X, CenterFlatPoint.Y, EndPoint.Z - StartPoint.Z);
            }

            const string ENoArcCenter = "Could not find a suitable center for arc";

            if (CenterPoint.X <= double.MinValue) throw new Exception(ENoArcCenter);
            if (CenterPoint.Y <= double.MinValue) throw new Exception(ENoArcCenter);

            Initialize();
        }

        public RadialInterpolation(Point3D startPoint, Point3D centerPoint, Point3D endPoint, RadialInterpolationDirection direction)
        {
            StartPoint = startPoint;
            StartFlatPoint = new Point(startPoint.X, startPoint.Y);
            CenterPoint = centerPoint;
            CenterFlatPoint = new Point(centerPoint.X, centerPoint.Y);
            EndPoint = endPoint;
            EndFlatPoint = new Point(endPoint.X, endPoint.Y);
            Direction = direction;

            Initialize();
        }

        private void Initialize()
        {
            // Distance from start to center. 
            double orx = StartPoint.X - CenterPoint.X;
            double ory = StartPoint.Y - CenterPoint.Y;

            double enx = EndPoint.X - CenterPoint.X;
            double eny = EndPoint.Y - CenterPoint.Y;

            ArcRadius = Math.Sqrt(orx * orx + ory * ory);

            // Alpha angle: start with X axis
            Alpha = Math.Atan2(ory, orx);

            // Beta angle: end with X axis
            Beta = Math.Atan2(eny, enx);

            // ArcAngle (beta - alpha) 
            if (Alpha < 0 && Beta > 0)
                ArcAngleRad = Beta - (Alpha + (2 * Math.PI));
            else if (Alpha > 0 && Beta < 0)
                ArcAngleRad = (Beta + (2 * Math.PI)) - Alpha;
            else
                ArcAngleRad = Beta - Alpha;

            if (Math.Abs(ArcAngleRad) > Math.PI)
                ArcAngleRad = Beta - Alpha;

            ArcAngleDeg = Math.Abs(ArcAngleRad * 180 / Math.PI);
        }

        public Point3D GetArcPointDeg(double angle)
        {
            var anglee = Math.Abs(angle);

            if (ArcAngleDeg < angle) anglee = ArcAngleDeg;

            return Direction == RadialInterpolationDirection.ClockWise ? GetArcPointRad(Math.PI * (anglee * -1) / 180) : GetArcPointRad(Math.PI * anglee / 180);
        }

        public Point3D GetArcPointRad(double angle)
        {
            if (angle == 0) return StartPoint;

            double anglee;

            if (Direction == RadialInterpolationDirection.CounterClockWise)
                anglee = angle + Alpha;
            else
                anglee = angle - Alpha;

            double x = CenterPoint.X + ArcRadius * Math.Cos(anglee);
            double y = CenterPoint.Y + ArcRadius * Math.Sin(anglee);
            double z = angle / ArcAngleRad * (EndPoint.Z - StartPoint.Z);
            var result = new Point3D(x, y, z);

            return result;
        }

        public List<Point3D> GetArcPoints(double stepDistance, double angleIncrement)
        {
            var points = new List<Point3D>();
            points.Add(StartPoint);

            for (double i = 0; i < ArcAngleDeg; i += angleIncrement)
            {
                var point = GetArcPointDeg(i);

                if (Math.Abs(point.X - points.Last().X) > stepDistance || Math.Abs(point.Y - points.Last().Y) > stepDistance || Math.Abs(point.Z - points.Last().Z) > stepDistance)
                    points.Add(point);
            }

            points.Add(EndPoint);
            return points;
        }
    }
}
