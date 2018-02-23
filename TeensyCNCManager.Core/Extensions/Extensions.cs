namespace TeensyCNCManager.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media.Media3D;

    public static class Extensions
    {
        public static int ToInt(this byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToInt32(bytes, 0);
        }

        public static int ToInt(this IEnumerable<byte> bytes)
        {
            return bytes.ToArray().ToInt();
        }

        public static IEnumerable<TResult> SelectWithPrevious<TSource, TResult>
    (this IEnumerable<TSource> source,
     Func<TSource, TSource, TResult> projection)
        {
            using (var iterator = source.GetEnumerator())
            {
                if (!iterator.MoveNext())
                {
                    yield break;
                }
                TSource previous = iterator.Current;
                while (iterator.MoveNext())
                {
                    yield return projection(previous, iterator.Current);
                    previous = iterator.Current;
                }
            }
        }

        private const double DegToRad = Math.PI / 180;

        public static Vector3D Rotate(this Vector3D v, double degrees)
        {
            return v.RotateRadians(degrees * DegToRad);
        }

        public static Vector3D RotateRadians(this Vector3D v, double radians)
        {
            var ca = Math.Cos(radians);
            var sa = Math.Sin(radians);
            return new Vector3D(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y, v.Z);
        }

        public static Vector Rotate(this Vector v, double degrees)
        {
            return v.RotateRadians(degrees * DegToRad);
        }

        public static Vector RotateRadians(this Vector v, double radians)
        {
            var ca = Math.Cos(radians);
            var sa = Math.Sin(radians);
            return new Vector(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y);
        }
    }
}
