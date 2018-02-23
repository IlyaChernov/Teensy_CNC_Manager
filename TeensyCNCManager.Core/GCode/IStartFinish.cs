namespace TeensyCNCManager.Core.GCode
{
   public interface IStartFinish
    {
        double? XStart
        {
            get;
            set;
        }

        double? YStart
        {
            get;
            set;
        }

        double? ZStart
        {
            get;
            set;
        }

        double? XDestination
        {
            get;
            set;
        }

        double? YDestination
        {
            get;
            set;
        }

        double? ZDestination
        {
            get;
            set;
        }
    }
}
