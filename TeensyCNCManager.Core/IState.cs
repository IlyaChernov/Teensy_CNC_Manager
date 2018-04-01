namespace TeensyCNCManager.Core
{
    using System.Collections.Generic;
    using TeensyCNCManager.Core.Extensions;

    public interface IState
    {
        bool IsConnected { get; set; }

        bool IsRunning { get; set; }

        List<string> GCode { get; set; }
        List<string> PostedGCode { get; set; }

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
       
        int DeviceQueueLength { get; set; }

        int DeviceLineNumber { get; set; }

        EngineState DeviceEngineState { get; set; }       

        decimal StepsToDistance(long steps);
    }
}
