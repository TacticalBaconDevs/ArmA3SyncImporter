namespace ArmA3SyncImporter.model
{
    public class PboFile
    {
        public string name;
        public long size = 0;

        public PboFile(string name)
        {
            this.name = name;
        }

        public PboFile(FileInfo fileInfo)
        {
            name = fileInfo.Name;
            size = fileInfo.Length;
        }

    }
}
