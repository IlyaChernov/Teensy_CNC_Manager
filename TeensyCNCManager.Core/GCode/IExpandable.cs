namespace TeensyCNCManager.Core.GCode
{
    using System.Collections.Generic;

    public interface IExpandable
    {
        IEnumerable<IGcode> Expand(IGcode defaultCommand, double defaultSpeed, double stepSize, double angle);
    }
}
