namespace TeensyCNCManager.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Xml.Serialization;
    using TeensyCNCManager.Core.Extensions;
    using TeensyCNCManager.Core.GCode;

    [Serializable]
    public class GlobalState : IState
    {
        [XmlIgnore]
        public Dictionary<IGcode, decimal> GCodeSpeeds { get; set; }

        [XmlIgnore]
        public bool IsConnected { get; set; }

        [XmlIgnore]
        public bool IsRunning { get; set; }

        [XmlIgnore]
        public Queue<string> ProcessingGCode { get; set; }

        [XmlIgnore]
        public List<string> GCode { get; set; }

        [XmlIgnore]
        public List<string> PostedGCode { get; set; }

        [XmlIgnore]
        public long ProgressMaximum { get; set; }

        [XmlIgnore]
        public long Progress { get; set; }

        [XmlIgnore]
        public string FileName { get; set; }

        [XmlIgnore]
        public float XPosition { get; set; }

        [XmlIgnore]
        public float YPosition { get; set; }

        [XmlIgnore]
        public float ZPosition { get; set; }

        //[XmlIgnore]
        //public decimal APosition { get; set; }

        //[XmlIgnore]
        //public decimal BPosition { get; set; }

        //[XmlIgnore]
        //public decimal CPosition { get; set; }

        [XmlIgnore]
        public float XDestination { get; set; }

        [XmlIgnore]
        public float YDestination { get; set; }

        [XmlIgnore]
        public float ZDestination { get; set; }

        //[XmlIgnore]
        //public decimal ADestination { get; set; }

        //[XmlIgnore]
        //public decimal BDestination { get; set; }

        //[XmlIgnore]
        //public decimal CDestination { get; set; }

        [XmlIgnore]
        public int DeviceQueueLength { get; set; }

        [XmlIgnore]
        public int DeviceLineNumber { get; set; }

        [XmlIgnore]
        public EngineState DeviceEngineState { get; set; }

        [XmlIgnore]
        public FixedSizedQueue<string> Log { get; set; }

       // [XmlIgnore]
       // public List<SCodeLine> PreprocessedGCodes { get; set; }

        [XmlIgnore]
        public IGcode LastPreprocessedGCode { get; set; }

        [XmlIgnore]
        public IEnumerable<KeyValuePair<string, string>> HidDevices { get; set; }

        [XmlIgnore]
        public Point[] MovementPoints { get; set; }

        public string CNCDeviceHIDPath { get; set; }

        public double LeadscrewPitch { get; set; }

        public long StepsPerRevolution { get; set; }

        public double DefaultSpeed { get; set; }

        public double DefaultQueueSize { get; set; }

        //[XmlIgnore]
        //public long XPositionSteps { get; set; }

        //[XmlIgnore]
        //public long YPositionSteps { get; set; }

        //[XmlIgnore]
        //public long ZPositionSteps { get; set; }

        //[XmlIgnore]
        //public long APositionSteps { get; set; }

        //[XmlIgnore]
        //public long BPositionSteps { get; set; }

        //[XmlIgnore]
        //public long CPositionSteps { get; set; }

        //[XmlIgnore]
        //public long XDestinationSteps { get; set; }

        //[XmlIgnore]
        //public long YDestinationSteps { get; set; }

        //[XmlIgnore]
        //public long ZDestinationSteps { get; set; }

        //[XmlIgnore]
        //public long ADestinationSteps { get; set; }

        //[XmlIgnore]
        //public long BDestinationSteps { get; set; }

        //[XmlIgnore]
        //public long CDestinationSteps { get; set; }

        public GlobalState()
        {
            //PreprocessedGCodes = new List<SCodeLine>();
            PostedGCode = new List<string>();
            GCode = new List<string>();
        }

        public int? DistanceToSteps(double? distance)
        {
            return distance.HasValue ? DistanceToSteps(distance.Value) : int.MaxValue;
        }


        public int? DistanceToSteps(double distance)
        {
            return (int)(distance / (LeadscrewPitch / StepsPerRevolution));
        }

        public decimal StepsToDistance(long steps)
        {
            return (decimal)(((double)steps / StepsPerRevolution) * LeadscrewPitch);
        }

        public void Save()
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            foreach (var file in dir.EnumerateFiles("options.*.xml"))
            {
                file.Delete();
            }

            var formatter = new XmlSerializer(GetType());

            using (var fs = new FileStream($"options.{DateTimeOffset.Now.ToUnixTimeSeconds()}.xml", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, this);
            }
        }

        public static GlobalState Load()
        {
            var result = new GlobalState();
            var formatter = new XmlSerializer(typeof(GlobalState));

            string path = Directory.GetCurrentDirectory();
            var directory = new DirectoryInfo(path);
            if (directory.GetFiles().Where(x => x.Name.StartsWith("options.")).Count() > 0)
            {
                var myFile = (from f in directory.GetFiles()
                              where f.Name.StartsWith("options.")
                              orderby f.LastWriteTime descending
                              select f).First();

                if (File.Exists(myFile.Name))
                    using (var fs = new FileStream(myFile.Name, FileMode.Open))
                    {

                        try
                        {
                            result = (GlobalState)formatter.Deserialize(fs);
                        }
                        catch { }

                    }
            }
            return result;
        }
    }
}
