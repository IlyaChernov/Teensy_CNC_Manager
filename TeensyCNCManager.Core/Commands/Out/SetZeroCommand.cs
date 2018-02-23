namespace TeensyCNCManager.Core.Commands
{
    using System.IO;

    public class SetZeroCommand : BaseCommand
    {
        private int dimension;

        public SetZeroCommand()
        {
            CommandCode = 272;
        }       

        public int Dimension
        {
            get
            {
                return dimension;
            }
            set
            {
                dimension = value;
            }
        }

        protected override MemoryStream WritePayload(MemoryStream stream)
        {
            var str = base.WritePayload(stream);
            var writer = new BinaryWriter(str);

            writer.Write(dimension);

            return str;
        }
    }
}
