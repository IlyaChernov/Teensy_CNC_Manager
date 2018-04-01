namespace TeensyCncManager.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using HidLibrary;

    using TeensyCNCManager.Core;
    using TeensyCNCManager.Core.Commands;
    using TeensyCNCManager.Core.GCode;

    using TeensyCncManager.HidWrapper;
    using TeensyCncManager.ViewModelUtils;
    using TeensyCNCManager.Core.Extensions;

    public class CNCViewModel : ViewModelBase, IState
    {
        public readonly GlobalState Gs;

        private static HidDeviceWrapper cncDevice;

        private double speed;

        private float distance;

        public CNCViewModel()
        {
            Gs = GlobalState.Load();

            InitializeHID(Gs.CNCDeviceHIDPath);
            //Log 
            Gs.Log = new FixedSizedQueue<string> { Limit = 7 };
            Gs.ProcessingGCode = new Queue<string>();
            Gs.Log.QueueChanged += Log_QueueChanged;
            PropertyChanged += CNCViewModel_PropertyChanged;
        }

        private void InitializeHID(string devicePath)
        {
            cncDevice = new HidDeviceWrapper(devicePath);
            cncDevice.ReportReceived += cncDevice_ReportReceived;
            cncDevice.DeviceConnected += cncDevice_DeviceConnected;
            cncDevice.DeviceDisconnected += cncDevice_DeviceDisconnected;

            cncDevice.Initialize();
        }

        void CNCViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CNCDeviceHIDPath":
                case "StepsPerRevolution":
                case "LeadscrewPitch":
                case "DefaultSpeed":
                case "DefaultQueueSize":
                    Gs.Save();
                    break;
            }

        }

        void Log_QueueChanged()
        {
            OnPropertyChanged("Log");
        }

        void cncDevice_DeviceDisconnected()
        {
            IsConnected = false;
            AddToLog("Teensy CNC Disconnected");
        }

        void cncDevice_DeviceConnected()
        {
            IsConnected = true;
            cncDevice.StartCommunication(true);
            AddToLog("Teensy CNC Connected");
        }

        void cncDevice_ReportReceived(HidReport report)
        {
            var sc = new BaseCommand(report.Data);

            switch (sc.CommandCode)
            {
                case 65280:
                    var state = new StatusReport(report.Data);
                    state.Act(this);

                    if (Gs.IsRunning && Gs.IsConnected && Gs.DeviceQueueLength < Gs.DefaultQueueSize)
                    {
                        if (PostedGCode.Count > Progress)
                        {
                            cncDevice.SendReport(PostedGCode[(int)Progress]);
                            Gs.DeviceQueueLength += PostedGCode[(int)Progress].Length;
                            //var code = GParser.Parse(PostedGCode[(int)Progress], LastPreprocessedGCode ?? new G00());
                            //LastPreprocessedGCode = code;                            
                            Progress++;                            
                        }
                        else
                        {
                            Gs.IsRunning = false;
                            Gs.Progress = 0;
                        }
                    }

                    if (!Gs.IsRunning && ProcessingGCodeList.Any())
                    {
                        var str = ProcessingGCodeList.Dequeue();
                        cncDevice.SendReport(str);
                        Gs.DeviceQueueLength += str.Length;
                    }

                    return;

                case 65282:
                    var posstate = new PositionsReport(report.Data);
                    posstate.Act(this);

                    return;
            }
        }

        public IEnumerable<KeyValuePair<string, string>> HIDDevices
        {
            get
            {
                return Gs.HidDevices;
            }
            set
            {
                Gs.HidDevices = value;
                OnPropertyChangedAuto();
            }
        }

        public string CNCDeviceHIDPath
        {
            get
            {
                return Gs.CNCDeviceHIDPath;
            }
            set
            {
                if (Gs.CNCDeviceHIDPath != value)
                {
                    InitializeHID(value);
                }

                Gs.CNCDeviceHIDPath = value;
                OnPropertyChangedAuto();
            }
        }

        public Point[] MovementPoints
        {
            get
            {
                return Gs.MovementPoints;
            }
            set
            {
                Gs.MovementPoints = value;
                OnPropertyChangedAuto();
            }
        }

        public ICommand ZeroDimensionCommand
        {
            get
            {
                return new RelayCommand(ZeroDimension, o => CanCommand);
            }
        }

        public ICommand StartPauseWorkCommand
        {
            get
            {
                return new RelayCommand(StartPauseWork, o => CanStartPause);
            }
        }

        public ICommand StopWorkCommand
        {
            get
            {
                return new RelayCommand(StopWork, o => CanStop);
            }
        }

        public ICommand ManualGCodeMinusCommand
        {
            get
            {
                return new RelayCommand(ManualGCodeMinus, o => CanCommand);
            }
        }

        public ICommand ManualGCodeMinusPlusCommand
        {
            get
            {
                return new RelayCommand(ManualGCodeMinusPlus, o => CanCommand);
            }
        }

        public ICommand ManualGCodePlusMinusCommand
        {
            get
            {
                return new RelayCommand(ManualGCodePlusMinus, o => CanCommand);
            }
        }

        public ICommand ManualGCodePlusCommand
        {
            get
            {
                return new RelayCommand(ManualGCodePlus, o => CanCommand);
            }
        }

        public ICommand ManualHomeGCodeCommand
        {
            get
            {
                return new RelayCommand(ManualHomeGCode, o => CanCommand);
            }
        }

        public bool CanCommand
        {
            get
            {
                return IsConnected && !IsRunning && DeviceEngineState != EngineState.EmergencyStopped &&
                    XPosition == XDestination && YPosition == YDestination && ZPosition == ZDestination;
                //&&
                //  APosition == ADestination && BPosition == BDestination && CPosition == CDestination;
            }
        }

        public bool CanStartPause
        {
            get
            {
                return (IsConnected && PostedGCode.Any() && DeviceEngineState == EngineState.Running) || IsRunning;
            }
        }

        public bool CanStop
        {
            get
            {
                return IsRunning;
            }
        }


        public bool IsRunning
        {
            get { return Gs.IsRunning; }

            set
            {
                Gs.IsRunning = value;
                OnPropertyChangedAuto();
            }
        }

        public bool IsConnected
        {
            get { return Gs.IsConnected; }

            set
            {
                Gs.IsConnected = value;
                OnPropertyChangedAuto();
            }
        }

        public List<string> GCode
        {
            get
            {
                return Gs.GCode;
            }
            set
            {
                Gs.GCode = value;
                OnPropertyChangedAuto();
            }
        }

        public List<string> PostedGCode
        {
            get
            {
                return Gs.PostedGCode;
            }
            set
            {
                Gs.PostedGCode = value;
                OnPropertyChangedAuto();
            }
        }

        public long ProgressMaximum
        {
            get
            {
                return Gs.ProgressMaximum;
            }
            set
            {
                Gs.ProgressMaximum = value;
                OnPropertyChangedAuto();
            }
        }

        public long Progress
        {
            get
            {
                return Gs.Progress;
            }
            set
            {
                Gs.Progress = value;
                OnPropertyChangedAuto();
            }
        }

        public string FileName
        {
            get
            {
                return Gs.FileName;
            }
            set
            {
                Gs.FileName = value;
                OnPropertyChangedAuto();
            }
        }

        public float XPosition
        {
            get
            {
                return Gs.XPosition;
            }
            set
            {
                if (Gs.XPosition != value)
                {
                    Gs.XPosition = value;
                    OnPropertyChangedAuto();
                }
            }
        }

        public float YPosition
        {
            get
            {
                return Gs.YPosition;
            }
            set
            {
                if (Gs.YPosition != value)
                {
                    Gs.YPosition = value;
                    OnPropertyChangedAuto();
                }
            }
        }

        public float ZPosition
        {
            get
            {
                return Gs.ZPosition;
            }
            set
            {
                if (Gs.ZPosition != value)
                {
                    Gs.ZPosition = value;
                    OnPropertyChangedAuto();
                }
            }
        }

        //public decimal APosition
        //{
        //    get
        //    {
        //        return Gs.APosition;
        //    }
        //    set
        //    {
        //        if (Gs.APosition != value)
        //        {
        //            Gs.APosition = value;
        //            OnPropertyChangedAuto();
        //        }
        //    }
        //}

        //public decimal BPosition
        //{
        //    get
        //    {
        //        return Gs.BPosition;
        //    }
        //    set
        //    {
        //        if (Gs.BPosition != value)
        //        {
        //            Gs.BPosition = value;
        //            OnPropertyChangedAuto();
        //        }
        //    }
        //}

        //public decimal CPosition
        //{
        //    get
        //    {
        //        return Gs.CPosition;
        //    }
        //    set
        //    {
        //        if (Gs.CPosition != value)
        //        {
        //            Gs.CPosition = value;
        //            OnPropertyChangedAuto();
        //        }
        //    }
        //}

        public float XDestination
        {
            get
            {
                return Gs.XDestination;
            }
            set
            {
                Gs.XDestination = value; OnPropertyChangedAuto();
            }
        }

        public float YDestination
        {
            get
            {
                return Gs.YDestination;
            }
            set
            {
                Gs.YDestination = value; OnPropertyChangedAuto();
            }
        }

        public float ZDestination
        {
            get
            {
                return Gs.ZDestination;
            }
            set
            {
                Gs.ZDestination = value; OnPropertyChangedAuto();
            }
        }

        //public decimal ADestination
        //{
        //    get
        //    {
        //        return Gs.ADestination;
        //    }
        //    set
        //    {
        //        Gs.ADestination = value; OnPropertyChangedAuto();
        //    }
        //}

        //public decimal BDestination
        //{
        //    get
        //    {
        //        return Gs.BDestination;
        //    }
        //    set
        //    {
        //        Gs.BDestination = value; OnPropertyChangedAuto();
        //    }
        //}

        //public decimal CDestination
        //{
        //    get
        //    {
        //        return Gs.CDestination;
        //    }
        //    set
        //    {
        //        Gs.CDestination = value; OnPropertyChangedAuto();
        //    }
        //}

        public int DeviceQueueLength
        {
            get
            {
                return Gs.DeviceQueueLength;
            }
            set
            {
                if (Gs.DeviceQueueLength != value)
                {
                    Gs.DeviceQueueLength = value;
                    OnPropertyChangedAuto();
                }
            }
        }

        public int DeviceLineNumber
        {
            get
            {
                return Gs.DeviceLineNumber;
            }
            set
            {
                if (Gs.DeviceLineNumber != value)
                {
                    AddToLog("Processing line " + value);
                    Gs.DeviceLineNumber = value;
                    OnPropertyChangedAuto();
                }
            }
        }

        public EngineState DeviceEngineState
        {
            get
            {
                return Gs.DeviceEngineState;
            }
            set
            {
                if (Gs.DeviceEngineState != value)
                {
                    Gs.DeviceEngineState = value;
                    OnPropertyChangedAuto();
                }
            }
        }

        public double Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
                OnPropertyChangedAuto();
            }
        }

        public float Distance
        {
            get
            {
                return distance;
            }
            set
            {
                distance = value;
                OnPropertyChangedAuto();
            }
        }

        public Queue<string> ProcessingGCodeList
        {
            get
            {
                return Gs.ProcessingGCode;
            }
            set
            {
                Gs.ProcessingGCode = value;
                OnPropertyChangedAuto();
            }
        }

        public List<string> Log
        {
            get
            {
                return Gs.Log.ToList();
            }
        }

       // public IGcode LastPreprocessedGCode
        //{
        //    get
        //    {
        //        return Gs.LastPreprocessedGCode;
        //    }
        //    set
        //    {
        //        Gs.LastPreprocessedGCode = value;
        //        OnPropertyChangedAuto();
        //    }
        //}

        public long StepsPerRevolution
        {
            get
            {
                return Gs.StepsPerRevolution;
            }
            set
            {
                Gs.StepsPerRevolution = value;
                OnPropertyChangedAuto();
            }
        }

        public double LeadscrewPitch
        {
            get
            {
                return Gs.LeadscrewPitch;
            }
            set
            {
                Gs.LeadscrewPitch = value;
                OnPropertyChangedAuto();
            }
        }

        public double DefaultSpeed
        {
            get
            {
                return Gs.DefaultSpeed;
            }
            set
            {
                Gs.DefaultSpeed = value;
                OnPropertyChangedAuto();
            }
        }

        public double DefaultQueueSize
        {
            get
            {
                return Gs.DefaultQueueSize;
            }
            set
            {
                Gs.DefaultQueueSize = value;
                OnPropertyChangedAuto();
            }
        }

        private void ZeroDimension(object obj)
        {
            var szk = new SetZeroCommand { LineNumber = 0, Dimension = (int)obj };

            cncDevice.SendReport(szk.GetDataBytes());
            AddToLog($"Zero position is set for {obj} dimension");
        }

        private void StartPauseWork(object obj)
        {
            Gs.IsRunning = !Gs.IsRunning;

            var state = Gs.IsRunning ? "started" : "paused";
            AddToLog($"GCode operations {state}");
        }

        private void StopWork(object obj)
        {
            Gs.IsRunning = false;

            Gs.Progress = 0;

            AddToLog("GCode operations stopped");
        }

        private void ManualGCodeMinus(object obj)
        {
            ProcessingGCodeList.Clear();
            ProcessingGCodeList.Enqueue(string.Format(((string)obj), XPosition - Distance, YPosition - Distance, ZPosition - Distance, Speed));
        }
        private void ManualGCodePlus(object obj)
        {
            ProcessingGCodeList.Clear();
            ProcessingGCodeList.Enqueue(string.Format(((string)obj), XPosition + Distance, YPosition + Distance, ZPosition + Distance, Speed));
        }

        private void ManualGCodePlusMinus(object obj)
        {
            ProcessingGCodeList.Clear();
            ProcessingGCodeList.Enqueue(string.Format(((string)obj),XPosition + Distance, YPosition - Distance, 0, Speed));
        }

        private void ManualGCodeMinusPlus(object obj)
        {
            ProcessingGCodeList.Clear();
            ProcessingGCodeList.Enqueue(string.Format(((string)obj), XPosition - Distance, YPosition + Distance, 0, Speed));
        }

        private void ManualHomeGCode(object obj)
        {
            ProcessingGCodeList.Clear();
            ProcessingGCodeList.Enqueue(string.Format(((string)obj), 0, 0, 0, Speed));
        }

        public void AddToLog(string msg)
        {
            Gs.Log.EnqueueWithLimit(DateTime.Now.ToLongTimeString() + " : " + msg);
        }

        public decimal StepsToDistance(long steps)
        {
            return Gs.StepsToDistance(steps);
        }
    }
}