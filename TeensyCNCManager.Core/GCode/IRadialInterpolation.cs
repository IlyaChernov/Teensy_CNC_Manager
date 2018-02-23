namespace TeensyCNCManager.Core.GCode
{
    interface IRadialInterpolation
   {       
        double? IDistance
        {
            get;
            set;
        }

        double? JDistance
        {
            get;
            set;
        }

        double? KDistance
        {
            get;
            set;
        }

        double? RDistance
        {
            get;
            set;
        }
    }
}
