namespace ArmA3SyncImporter.model
{
    public class A3SEvent
    {
        public string EventName { get; set; } = "";

        public List<string> ModNames { get; set; } = new();

        public override string ToString() => EventName;

    }

}
