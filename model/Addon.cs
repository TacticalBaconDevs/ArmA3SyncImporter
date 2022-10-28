namespace ArmA3SyncImporter.model
{
    public class Addon
    {
        public string AddonName { get; set; } = "";

        public List<PboFile> Files { get; set; } = new();

    }
}
