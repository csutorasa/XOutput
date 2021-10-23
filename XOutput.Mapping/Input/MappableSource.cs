namespace XOutput.Mapping.Input
{
    public class MappableSource
    {
        public int Id => id;
        public double Value { get; set; }

        private readonly int id;

        public MappableSource(int id)
        {
            this.id = id;
        }
    }
}
