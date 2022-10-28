using ArmA3SyncImporter.impl;
using ArmA3SyncImporter.model;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ArmA3SyncImporter
{
    public class ImporterUtils
    {
        private const string EXPORT_JAR = "ArmA3SyncExporter_20221009.jar";
        private const string START_POINT = "-----------------START-----------------";
        private const string END_POINT = "-----------------END-----------------";

        private static void Init()
        {
            if (!File.Exists(EXPORT_JAR))
                LoadFileFromResource(EXPORT_JAR);
        }

        private static void LoadFileFromResource(string fileName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string? resourceName = assembly.GetManifestResourceNames().FirstOrDefault(rn => rn.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));
            if (resourceName == null)
                throw new Exception(string.Format("'{0}' was not found in resources!", fileName));

            foreach (string file in Directory.EnumerateFiles("", "ArmA3SyncExporter*.jar"))
                File.Delete(file);

            using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
            using (Stream file = File.Create(fileName))
            {
                if (stream != null)
                    stream.CopyTo(file);
            }

            if (!File.Exists(EXPORT_JAR))
                throw new Exception(string.Format("'{0}' was not found on disk!", fileName));
        }

        public static List<string> StartImport(string url)
        {
            Init();

            // create an hidden process
            Process process = new();
            process.StartInfo = new("javaw.exe")
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = string.Format("-Djava.net.preferIPv4Stack=true -jar {0} -console \"{1}\"", EXPORT_JAR, url),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            process.Start();
            process.WaitForExit();
            int exitCode = process.ExitCode;

            // get output of process
            List<string> output = new();
            while (!process.StandardOutput.EndOfStream)
                output.Add(string.Format("{0}", process.StandardOutput.ReadLine()));
            while (!process.StandardError.EndOfStream)
                output.Add(string.Format("Error: {0}", process.StandardError.ReadLine()));

            // parse output
            List<string>? parsedOutput = ParseStandardOutput(output);
            if (parsedOutput == null)
                throw new Exception(string.Format("exitCode: {0}{1}{2}", exitCode, Environment.NewLine, string.Join(Environment.NewLine, output)));

            return parsedOutput;
        }

        private static List<string>? ParseStandardOutput(List<string> output)
        {
            int start = output.IndexOf(START_POINT);
            int end = output.IndexOf(END_POINT);

            return start != -1 && end != -1 ? output.GetRange(start, end) : null;
        }

        public static List<Addon?> GetRemoteSync(string autoConfig)
        {
            return StartImport(autoConfig.Replace("/autoconfig", "/sync"))
                    .Select(x => RepoManager.GetAddonFromLine(x))
                    .Where(x => x != null).ToList();
        }

        public static List<Addon?> GetLokalSync(string addonsFolder)
        {
            return Directory.GetDirectories(addonsFolder, "@*", SearchOption.TopDirectoryOnly)
                    .Select(x => RepoManager.GetAddonFromPath(x))
                    .Where(x => x != null).ToList();
        }

        public static List<Event?> GetEvent(string autoConfig)
        {
            return StartImport(autoConfig.Replace("/autoconfig", "/events"))
                    .Select(x => RepoManager.GetEventFromLine(x))
                    .Where(x => x != null).ToList();
        }

    }
}