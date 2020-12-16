using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;

namespace Updater
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var sourceFolder = args[0];
            var destinamtionFolder = args[1];
            var exe = args[2];

            foreach (var file in Directory.GetFiles(sourceFolder))
            {
                using (var source = await GetFileStream(file, FileMode.Open, FileAccess.Read))
                {
                    var dstPath = Path.Combine(destinamtionFolder, Path.GetFileName(file));
                    FileMode mode;
                    if (File.Exists(dstPath))
                    {
                        mode = FileMode.Truncate;
                    }
                    else
                    {
                        mode = FileMode.CreateNew;
                    }

                    using var dst = await GetFileStream(dstPath, mode, FileAccess.ReadWrite);

                    Console.WriteLine("Copy file: " + Path.GetFileName(file));
                    await source.CopyToAsync(dst);
                }
                File.Delete(file);
            }
            
            var proc = new ProcessStartInfo(exe);
            Process.Start(proc);
            Environment.Exit(0);
        }

        static async Task<FileStream> GetFileStream(string file, FileMode mode, FileAccess access, CancellationToken cancellationToken = default)
        {
            while (true)
            {
                Console.WriteLine("Waiting for file: " + file);
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                try
                {
                    return File.Open(file, mode, access, FileShare.None);
                }
                catch(IOException)
                {
                    await Task.Delay(100);
                }
            }
        }

    }
}
