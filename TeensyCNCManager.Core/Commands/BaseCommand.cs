namespace TeensyCNCManager.Core.Commands
{
    using System.IO;

    public class BaseCommand
    {
        protected int PayloadReadCounter;

        protected byte[] DataBytes;

        public BaseCommand()
        {
        }

        public BaseCommand(byte[] dataBytes)
        {
            DataBytes = dataBytes;
            ReadDataBytes();
        }

        public int CommandCode { get; set; }

        public int LineNumber { get; set; }

        protected void ReadHeader()
        {
            var reader = new BinaryReader(new MemoryStream(DataBytes));

            CommandCode = reader.ReadInt32();
            LineNumber = reader.ReadInt32();
        }

        protected virtual MemoryStream WriteHeader(MemoryStream stream)
        {
            var writer = new BinaryWriter(stream);

            writer.Write(CommandCode);
            writer.Write(LineNumber);

            return stream;
        }

        private void ReadDataBytes()
        {
            ReadHeader();
            ReadPayload();
        }

        public byte[] GetDataBytes()
        {
            return WritePayload(WriteHeader(new MemoryStream(new byte[64]))).ToArray();
        }

        protected virtual void ReadPayload()
        {
        }

        protected virtual MemoryStream WritePayload(MemoryStream stream)
        {
            return stream;
        }

        public virtual void Act(IState gs) { }
    }
}
