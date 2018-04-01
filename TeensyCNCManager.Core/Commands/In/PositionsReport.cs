namespace TeensyCNCManager.Core.Commands
{
    using System.IO;
    using System.Linq;

    public class PositionsReport : BaseCommand
    {
        public PositionsReport(byte[] dataBytes) : base(dataBytes) { }

        public float XSteps { get; set; }

        public float YSteps { get; set; }

        public float ZSteps { get; set; }

        //public int ASteps { get; set; }

        //public int BSteps { get; set; }

        //public int CSteps { get; set; }


        public float XDestinationSteps { get; set; }

        public float YDestinationSteps { get; set; }

        public float ZDestinationSteps { get; set; }

        //public int ADestinationSteps { get; set; }

        //public int BDestinationSteps { get; set; }

        //public int CDestinationSteps { get; set; }
       
        protected override void ReadPayload()
        {
            var reader = new BinaryReader(new MemoryStream(DataBytes.Skip((sizeof(int) * 1)).ToArray()));

            XSteps = reader.ReadSingle(); //.ReadInt32();
            YSteps = reader.ReadSingle();
            ZSteps = reader.ReadSingle();
            //ASteps = reader.ReadInt32();
            //BSteps = reader.ReadInt32();
            //CSteps = reader.ReadInt32();

            XDestinationSteps = reader.ReadSingle();
            YDestinationSteps = reader.ReadSingle();
            ZDestinationSteps = reader.ReadSingle();
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
            gs.XPosition = XSteps; // gs.StepsToDistance(XSteps);
            gs.YPosition = YSteps;
            gs.ZPosition =ZSteps;
            //gs.APosition = gs.StepsToDistance(ASteps);
            //gs.BPosition = gs.StepsToDistance(BSteps);
            //gs.CPosition = gs.StepsToDistance(CSteps);
            
            gs.XDestination = XDestinationSteps;
            gs.YDestination = YDestinationSteps;
            gs.ZDestination = ZDestinationSteps;
            //gs.ADestination = gs.StepsToDistance(ADestinationSteps);
            //gs.BDestination = gs.StepsToDistance(BDestinationSteps);
            //gs.CDestination = gs.StepsToDistance(CDestinationSteps);
        }
    }
}
