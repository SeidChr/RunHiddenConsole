namespace PowerShellWindowHost
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;

    public static class Program
    {
        public static int Main(string[] args)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            var fullName = Path.GetFileName(executingAssembly.Location);
            
            var match = Regex.Match(fullName, @"(.+)w(\.\w{1,3})");
            if (!match.Success)
            {
                return -7001;
            }

            var targetExecutableName = match.Groups[1].Value + match.Groups[2].Value;

            var fullDirectory = Path.GetDirectoryName(executingAssembly.Location) ?? string.Empty;
            var executable = Path.Combine(fullDirectory, targetExecutableName);

            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = Directory.GetCurrentDirectory(),
                Arguments = string.Join(" ", args),
                FileName = executable,
            };

            try
            {
                var proc = Process.Start(startInfo);
                if (proc == null)
                {
                    return -7002;
                }

                proc.WaitForExit();
                return proc.ExitCode;
            }
            catch (Win32Exception)
            {
                return -7003;
            }
            catch (SystemException)
            {
                return -7004;
            }
        }
    }
}
