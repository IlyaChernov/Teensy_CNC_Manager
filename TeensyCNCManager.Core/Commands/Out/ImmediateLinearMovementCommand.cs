namespace TeensyCNCManager.Core.Commands
{
    using System.IO;

    public class ImmediateLinearMovementCommand : BaseCommand
    {
        private int? xPos;
        private int? yPos;
        private int? zPos;
        private int? aPos;
        private int? bPos;
        private int? cPos;

        private double? xyzSpeed;

        private double? abcSpeed;

        public ImmediateLinearMovementCommand()
        {
            CommandCode = 256;
        }

        public int? XPos
        {
            get
            {
                return xPos;
            }
            set
            {
                xPos = value;
            }
        }

        public int? YPos
        {
            get
            {
                return yPos;
            }
            set
            {
                yPos = value;
            }
        }

        public int? ZPos
        {
            get
            {
                return zPos;
            }
            set
            {
                zPos = value;
            }
        }

        public int? APos
        {
            get
            {
                return aPos;
            }
            set
            {
                aPos = value;
            }
        }

        public int? BPos
        {
            get
            {
                return bPos;
            }
            set
            {
                bPos = value;
            }
        }

        public int? CPos
        {
            get
            {
                return cPos;
            }
            set
            {
                cPos = value;
            }
        }

        public double? XYZSpeed
        {
            get
            {
                return xyzSpeed;
            }
            set
            {
                xyzSpeed = value;
            }
        }

        public double? ABCSpeed
        {
            get
            {
                return abcSpeed;
            }
            set
            {
                abcSpeed = value;
            }
        }

        protected override MemoryStream WriteHeader(MemoryStream stream)
        {
            var writer = new BinaryWriter(stream);
            writer.Write(CommandCode);
            return stream;
        }

        protected override MemoryStream WritePayload(MemoryStream stream)
        {
            var str = base.WritePayload(stream);
            var writer = new BinaryWriter(str);

            writer.Write(xPos.HasValue ? xPos.Value : int.MaxValue);
            writer.Write(yPos.HasValue ? yPos.Value : int.MaxValue);
            writer.Write(zPos.HasValue ? zPos.Value : int.MaxValue);
            writer.Write(aPos.HasValue ? aPos.Value : int.MaxValue);
            writer.Write(bPos.HasValue ? bPos.Value : int.MaxValue);
            writer.Write(cPos.HasValue ? cPos.Value : int.MaxValue);

            writer.Write((float)(xyzSpeed.HasValue ? xyzSpeed.Value : float.MaxValue));

            writer.Write((float)(abcSpeed.HasValue ? abcSpeed.Value : float.MaxValue));

            return str;
        }
    }
}
