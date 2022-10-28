namespace ArmA3SyncImporter.model
{
    public class A3SPboFile
    {
        public string PboName;
        public long Size = 0;

        public A3SPboFile(string name)
        {
            PboName = name;
        }

        public A3SPboFile(FileInfo fileInfo)
        {
            PboName = fileInfo.Name;
            Size = fileInfo.Length;
        }

        public override string ToString() => PboName;

    }
}
