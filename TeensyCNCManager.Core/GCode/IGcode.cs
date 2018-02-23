namespace TeensyCNCManager.Core.GCode
{
    public interface IGcode : IStartFinish, IMovementSpeed
    {
        void ClearParams();

        void WipeOutParams();

        void ApplyParam(string param);
    }
}
