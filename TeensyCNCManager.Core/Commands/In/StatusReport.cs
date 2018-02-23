namespace TeensyCNCManager.Core.Commands
{
    using System.IO;
    using System.Linq;

    public class StatusReport : BaseCommand
    {
        public StatusReport(byte[] dataBytes) : base(dataBytes) { }

        //public int LineNumber { get; set; }

        public EngineState EngineState { get; set; }

        public int QueueLength { get; set; }

        protected override void ReadPayload()
        {
            var reader = new BinaryReader(new MemoryStream(DataBytes.Skip((sizeof(int))).ToArray()));

            LineNumber = reader.ReadInt32();
            QueueLength = reader.ReadInt32();
            EngineState = (EngineState)reader.ReadInt32();            
        }

        protected override MemoryStream WritePayload(MemoryStream stream)
        {
            var str = base.WritePayload(stream);
            var writer = new BinaryWriter(str);


            writer.Write(LineNumber);
            writer.Write((int)EngineState);
            writer.Write(QueueLength);

            return str;
        }

        public override void Act(IState gs)
        {
            base.Act(gs);
            gs.DeviceEngineState = EngineState;
            gs.DeviceLineNumber = LineNumber;
            gs.DeviceQueueLength = QueueLength;

            if (EngineState == EngineState.EmergencyStopped)
            {
                gs.Progress = 0;
                gs.IsRunning = false;
            }
        }
    }
}
