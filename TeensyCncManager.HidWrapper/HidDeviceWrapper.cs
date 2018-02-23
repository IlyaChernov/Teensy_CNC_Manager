namespace TeensyCncManager.HidWrapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Text;
    using HidLibrary;

    public class HidDeviceWrapper
    {
        public delegate void ReportReceivedHandler(HidReport report);

        public event ReportReceivedHandler ReportReceived;

        public delegate void DeviceOpenedHandler();

        public event DeviceOpenedHandler DeviceOpened;

        public delegate void DeviceConnectedHandler();

        public event DeviceConnectedHandler DeviceConnected;

        public delegate void DeviceDisconnectedHandler();

        public event DeviceDisconnectedHandler DeviceDisconnected;

        private HidDevice device;

        private HidDevice Device
        {
            get
            {
                return device;
            }

            set
            {
                device = value;
                device.MonitorDeviceEvents = true;
                device.Inserted += hidDevice_Inserted;
                device.Removed += hidDevice_Removed;
            }
        }

        private readonly string devicePath;

        readonly Subject<Unit> stopSubject = new Subject<Unit>();

        public HidDeviceWrapper(string devicePath)
        {
            this.devicePath = devicePath;
        }

        public void Enumerate()
        {
           
        }

        public void SendReport(string report)
        {
            SendReport(Encoding.ASCII.GetBytes(report));
        }

        public void SendReport(byte[] bytes)
        {
            List<byte> localbytes = new List<byte>();
            byte[] initbytes = new byte[64];

            localbytes.InsertRange(0, initbytes);
            localbytes.InsertRange(0, bytes);

            if (Device != null && Device.IsConnected)
            {
                var bytess = localbytes.Take(63).ToList();
                bytess.Insert(0, 0);
                Device.WriteReport(new HidReport(64, new HidDeviceData(bytess.ToArray(), HidDeviceData.ReadStatus.Success)));
            }
            else throw new Exception();
        }

        public void Initialize()
        {          
            if (Device == null)
            {
                if (HidDevices.IsConnected(devicePath))
                {
                    Device = HidDevices.GetDevice(devicePath);
                }
                else
                {
                    Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1)).TakeUntil(stopSubject).Subscribe(
                    l =>
                    {
                        var dev = HidDevices.GetDevice(devicePath);

                        if (Device == null && dev != null)
                            Device = dev;
                    });
                }
            }
        }

        public void StartCommunication(bool force = false)
        {
            if (Device != null)
            {
                if (Device.IsConnected)
                {
                    if (!Device.IsOpen || force)
                    {
                        Device.OpenDevice();
                        DeviceOpened?.Invoke();
                        Device.ReadReport(OnReport);
                    }
                }
            }
            else
            {
                if (HidDevices.IsConnected(devicePath))
                {
                    Device = HidDevices.GetDevice(devicePath);

                    if (!Device.IsOpen || force)
                    {
                        Device.OpenDevice();
                        DeviceOpened?.Invoke();
                        Device.ReadReport(OnReport);
                    }
                }
                else
                {
                    Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1)).TakeUntil(stopSubject).Subscribe(
                    l =>
                    {
                        var dev = HidDevices.GetDevice(devicePath);

                        if (Device == null && dev != null)
                            Device = dev;

                        if (Device == null) return;

                        Device.OpenDevice();
                        DeviceOpened?.Invoke();
                        Device.ReadReport(OnReport);
                        stopSubject.OnNext(Unit.Default);
                    });
                }
            }
        }

        public void StopCommunication()
        {
            stopSubject.OnNext(Unit.Default);
            if (Device != null && Device.IsOpen) Device.CloseDevice();
        }

        void OnReport(HidReport report)
        {
            ReportReceived?.Invoke(report);

            if (Device.IsConnected && Device.IsOpen)
                Device.ReadReport(OnReport);
        }

        void hidDevice_Removed()
        {
            Device.CloseDevice();
            DeviceDisconnected?.Invoke();


        }

        void hidDevice_Inserted()
        {
            DeviceConnected?.Invoke();
        }
    }
}
