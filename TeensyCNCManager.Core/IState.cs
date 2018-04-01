namespace TeensyCNCManager.Core
{
    using System.Collections.Generic;
    using TeensyCNCManager.Core.Extensions;

    public interface IState
    {
        bool IsConnected { get; set; }

        bool IsRunning { get; set; }

        List<string> GCode { get; set; }

        long ProgressMaximum { get; set; }

        long Progress { get; set; }

        string FileName { get; set; }

        float XPosition { get; set; }

        float YPosition { get; set; }

        float ZPosition { get; set; }

        //decimal APosition { get; set; }

        //decimal BPosition { get; set; }

        //decimal CPosition { get; set; }

        float XDestination { get; set; }

        float YDestination { get; set; }

        float ZDestination { get; set; }

        //decimal ADestination { get; set; }

        //decimal BDestination { get; set; }

        //decimal CDestination { get; set; }

        //long XPositionSteps { get; set; }

        //long YPositionSteps { get; set; }

        //long ZPositionSteps { get; set; }

        //long APositionSteps { get; set; }

        //long BPositionSteps { get; set; }

        //long CPositionSteps { get; set; }

        //long XDestinationSteps { get; set; }

        //long YDestinationSteps { get; set; }

        //long ZDestinationSteps { get; set; }

        //long ADestinationSteps { get; set; }

        //long BDestinationSteps { get; set; }

        //long CDestinationSteps { get; set; }

        int DeviceQueueLength { get; set; }

        int DeviceLineNumber { get; set; }

        EngineState DeviceEngineState { get; set; }

        List<SCodeLine> PreprocessedGCodes { get; set; }

        decimal StepsToDistance(long steps);
    }
}
