namespace TeensyCNCManager.Core.GCode
{
    using TeensyCNCManager.Core.Interpolation;

    [Code(CodeName = "G03")]
    class G03 : G02
    {
        public new RadialInterpolationDirection Direction = RadialInterpolationDirection.CounterClockWise;
    }
}
