namespace TeensyCNCManager.Core.Commands
{
    using System.IO;
    using System.Linq;

    public class PositionsReport : BaseCommand
    {
        public PositionsReport(byte[] dataBytes) : base(dataBytes) { }

        public int XSteps { get; set; }

        public int YSteps { get; set; }

        public int ZSteps { get; set; }

        //public int ASteps { get; set; }

        //public int BSteps { get; set; }

        //public int CSteps { get; set; }


        public int XDestinationSteps { get; set; }

        public int YDestinationSteps { get; set; }

        public int ZDestinationSteps { get; set; }

        //public int ADestinationSteps { get; set; }

        //public int BDestinationSteps { get; set; }

        //public int CDestinationSteps { get; set; }
       
        protected override void ReadPayload()
        {
            var reader = new BinaryReader(new MemoryStream(DataBytes.Skip((sizeof(int) * 1)).ToArray()));

            XSteps = reader.ReadInt32();
            YSteps = reader.ReadInt32();
            ZSteps = reader.ReadInt32();
            //ASteps = reader.ReadInt32();
            //BSteps = reader.ReadInt32();
            //CSteps = reader.ReadInt32();

            XDestinationSteps = reader.ReadInt32();
            YDestinationSteps = reader.ReadInt32();
            ZDestinationSteps = reader.ReadInt32();
            //ADestinationSteps = reader.ReadInt32();
            //BDestinationSteps = reader.ReadInt32();
            //CDestinationSteps = reader.ReadInt32();
        }

        protected override MemoryStream WritePayload(MemoryStream stream)
        {
            var str = base.WritePayload(stream);
            var writer = new BinaryWriter(str);

            writer.Write(XSteps);
            writer.Write(YSteps);
            writer.Write(ZSteps);
            //writer.Write(ASteps);
            //writer.Write(BSteps);
            //writer.Write(CSteps);

            writer.Write(XDestinationSteps);
            writer.Write(YDestinationSteps);
            writer.Write(ZDestinationSteps);
            //writer.Write(ADestinationSteps);
            //writer.Write(BDestinationSteps);
            //writer.Write(CDestinationSteps);

            return str;
        }

        public override void Act(IState gs)
        {
            base.Act(gs);
            gs.XPosition = gs.StepsToDistance(XSteps);
            gs.YPosition = gs.StepsToDistance(YSteps);
            gs.ZPosition = gs.StepsToDistance(ZSteps);
            //gs.APosition = gs.StepsToDistance(ASteps);
            //gs.BPosition = gs.StepsToDistance(BSteps);
            //gs.CPosition = gs.StepsToDistance(CSteps);
            
            gs.XDestination = gs.StepsToDistance(XDestinationSteps);
            gs.YDestination = gs.StepsToDistance(YDestinationSteps);
            gs.ZDestination = gs.StepsToDistance(ZDestinationSteps);
            //gs.ADestination = gs.StepsToDistance(ADestinationSteps);
            //gs.BDestination = gs.StepsToDistance(BDestinationSteps);
            //gs.CDestination = gs.StepsToDistance(CDestinationSteps);

            gs.XPositionSteps = XSteps;
            gs.YPositionSteps = YSteps;
            gs.ZPositionSteps = ZSteps;
            //gs.APositionSteps = ASteps;
            //gs.BPositionSteps = BSteps;
            //gs.CPositionSteps = CSteps;

            gs.XDestinationSteps = XDestinationSteps;
            gs.YDestinationSteps = YDestinationSteps;
            gs.ZDestinationSteps = ZDestinationSteps;
            //gs.ADestinationSteps = ADestinationSteps;
            //gs.BDestinationSteps = BDestinationSteps;
            //gs.CDestinationSteps = CDestinationSteps;


        }
    }
}
