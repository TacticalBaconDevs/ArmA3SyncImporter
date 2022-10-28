namespace ArmA3SyncImporter.model
{
    public class A3SMod
    {
        public string ModName { get; set; } = "";

        public List<A3SPboFile> Files { get; set; } = new();

        public override string ToString() => ModName;

    }
}
