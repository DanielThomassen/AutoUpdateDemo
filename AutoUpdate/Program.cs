using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AutoUpdate
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var updatePath = "update";
            var downloadPath = "download";

            Console.WriteLine("Current version: " + GetVersion().ToString());
            while (true)
            {
                if (HasUpdate(updatePath))
                {
                    Console.WriteLine("Update found.");

                    await DownloadUpdate(updatePath, downloadPath);
                    Console.WriteLine("Update downloaded");

                    var proc = new ProcessStartInfo();
                    proc.WindowStyle = ProcessWindowStyle.Normal;
                    proc.CreateNoWindow = false;
                    proc.FileName = "updater.exe";
                    proc.Arguments = $"\"{Path.GetFullPath(downloadPath)}\" \"{Environment.CurrentDirectory}\" \"AutoUpdate.exe\"";
                    Process.Start(proc);
                    Environment.Exit(0);
                }
                else
                {
                    //Console.Write(". ");
                }
                

                await Task.Delay(TimeSpan.FromSeconds(3));
            }
            
        }

        private static async Task DownloadUpdate(string source, string destination)
        {
            foreach (var file in Directory.GetFiles(source))
            {
                var dstPath = Path.Combine(destination, Path.GetFileName(file));
                if (File.Exists(dstPath))
                {
                    File.Delete(dstPath);
                }
                File.Copy(file,dstPath);
            }
        }

        private static Version GetVersion(string file = "manifest.json")
        {
            var content = File.ReadAllText(file);
            var manifest = System.Text.Json.JsonSerializer.Deserialize<Manifest>(content);
            if (!Version.TryParse(manifest.Version, out var version))
            {
                return default;
            }

            return version;
        }

        private static bool HasUpdate(string path)
        {
            if (Directory.Exists(path))
            {
                var manifest = "manifest.json";
                if (!File.Exists(Path.Combine(path, manifest)))
                {
                    return false;
                }

                var currentVersion = GetVersion();
                var updateVersion = GetVersion(Path.Combine(path, manifest));

                return updateVersion > currentVersion;
            }
            return false;
        }
    }
}
