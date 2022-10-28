using ArmA3SyncImporter.impl;
using ArmA3SyncImporter.model;
using System.Diagnostics;
using System.Reflection;

namespace ArmA3SyncImporter
{
    public class ImporterUtils
    {
        private const string EXPORT_JAR = "ArmA3SyncExporter_20221012.jar";
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

            foreach (string file in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "ArmA3SyncExporter*.jar"))
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

            // get output of process
            List<string> output = process.StandardOutput.ReadToEnd().Split(Environment.NewLine).ToList();
            output.AddRange(process.StandardError.ReadToEnd().Split(Environment.NewLine).ToList());

            bool isExited = process.WaitForExit(5000);
            if (!isExited && !process.HasExited)
            {
                process.Kill();
                if (!process.WaitForExit(2000) && !process.HasExited)
                    throw new Exception("Process couldnt be killed!");
            }

            int exitCode = process.ExitCode;

            // parse output
            List<string>? parsedOutput = ParseStandardOutput(output);
            if (parsedOutput == null)
                throw new Exception(string.Format("exitCode: {0}{1}{2}", exitCode, Environment.NewLine, string.Join(Environment.NewLine, output)));

            return parsedOutput;
        }

        private static List<string>? ParseStandardOutput(List<string> output)
        {
            int start = output.IndexOf(START_POINT);
            int length = output.IndexOf(END_POINT) - start;

            return start > 0 && length > 0 ? output.GetRange(start + 1, length - 1).Where(x => x != null && x.Length > 0).ToList() : null;
        }

#pragma warning disable CS8619 // Die NULL-Zulässigkeit von Verweistypen im Wert entspricht nicht dem Zieltyp.

        public static List<A3SMod> GetRemoteMods(string autoConfig)
        {
            return StartImport(autoConfig.Replace("/autoconfig", "/sync"))
                    .Select(x => RepoManager.GetAddonFromLine(x))
                    .Where(x => x != null).ToList();
        }

        public static List<A3SMod> GetRemoteMods(string autoConfig, string eventName)
        {
            A3SEvent? selectedEvent = GetEvents(autoConfig).FirstOrDefault(x => x.EventName.Equals(eventName, StringComparison.OrdinalIgnoreCase));
            if (selectedEvent == null)
                throw new Exception(string.Format("The event: '{0}' is unknown", eventName));

            return GetRemoteMods(autoConfig, selectedEvent);
        }

        public static List<A3SMod> GetRemoteMods(string autoConfig, A3SEvent selectedEvent)
        {
            return StartImport(autoConfig.Replace("/autoconfig", "/sync"))
                    .Select(x => RepoManager.GetAddonFromLine(x))
                    .Where(x => x != null && selectedEvent.ModNames.Any(y => y.Equals(x.ModName, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
        }

        public static List<A3SMod> GetLocalMods(string addonsFolder)
        {
            return Directory.GetDirectories(addonsFolder, "@*", SearchOption.TopDirectoryOnly)
                    .Select(x => RepoManager.GetAddonFromPath(x))
                    .Where(x => x != null).ToList();
        }

        public static List<A3SEvent> GetEvents(string autoConfig)
        {
            return StartImport(autoConfig.Replace("/autoconfig", "/events"))
                    .Select(x => RepoManager.GetEventFromLine(x))
                    .Where(x => x != null).ToList();
        }

#pragma warning restore CS8619 // Die NULL-Zulässigkeit von Verweistypen im Wert entspricht nicht dem Zieltyp.

    }
}