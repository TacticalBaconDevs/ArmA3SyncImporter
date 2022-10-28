using ArmA3SyncImporter.model;

namespace ArmA3SyncImporter.impl
{
    internal class RepoManager
    {
        public static A3SMod? GetAddonFromLine(string line)
        {
            if (line == null || line.Length == 0 || !line.StartsWith("@"))
                return null;

            string[] values = line.Split("#", 2);
            if (values.Length != 2)
                return null;

            A3SMod addon = new();
            addon.ModName = values[0];
            addon.Files = new List<string>(values[1].Split("|")).Select(x => new A3SPboFile(x)).ToList();

            return addon;
        }

        public static A3SMod? GetAddonFromPath(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
                return null;

            A3SMod addon = new();
            addon.ModName = dirInfo.Name;

            string addonsFolder = Combine(dirInfo.FullName, "addons");
            if (Directory.Exists(addonsFolder))
            {
                List<string> fullFiles = Directory.GetFiles(addonsFolder, "*.pbo", SearchOption.TopDirectoryOnly).ToList();
                addon.Files = fullFiles.Select(x => new A3SPboFile(new FileInfo(x))).ToList();
            }

            return addon;
        }

        public static A3SEvent? GetEventFromLine(string line)
        {
            if (line == null || line.Length == 0)
                return null;

            string[] values = line.Split("#", 2);
            if (values.Length != 2)
                return null;

            A3SEvent eventEntry = new();
            eventEntry.EventName = values[0];
            eventEntry.ModNames = new List<string>(values[1].Split("|"));

            return eventEntry;
        }

        private static string Combine(string path1, string path2)
        {
            if (Path.IsPathRooted(path2))
            {
                path2 = path2.TrimStart(Path.DirectorySeparatorChar);
                path2 = path2.TrimStart(Path.AltDirectorySeparatorChar);
            }

            return Path.Combine(path1, path2);
        }

    }

}
