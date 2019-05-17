namespace PowerShellWindowHost
{
    using System;
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

            var settingsReader = new System.Configuration.AppSettingsReader();
            var logLevelString = settingsReader.GetValue("LogLevel", typeof(string)).ToString();
            if (Enum.TryParse(logLevelString, true, out SimpleLog.Severity logLevel))
            {
                SimpleLog.LogLevel = logLevel;
            }

            SimpleLog.Prefix = $"{fullName}.";

            SimpleLog.Info($"RunHiddenConsole Executing Assembly FullName: {fullName}");

            var match = Regex.Match(fullName, @"(.+)w(\.\w{1,3})");
            if (!match.Success)
            {
                SimpleLog.Error("Unable to find target executable name in own executable name.");
                return -7001;
            }

            var targetExecutableName = match.Groups[1].Value + match.Groups[2].Value;

            var fullDirectory = Path.GetDirectoryName(executingAssembly.Location) ?? string.Empty;
            var executable = Path.Combine(fullDirectory, targetExecutableName);

            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
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
                    SimpleLog.Error("Unable to start the target process.");
                    return -7002;
                }

                // process will close as soon as its waiting for interactive input.
                proc.StandardInput.Close();

                proc.WaitForExit();

                return proc.ExitCode;
            }
            catch (Exception ex)
            {
                SimpleLog.Error("Starting target process threw an unknown Exception: " + ex);
                SimpleLog.Log(ex);
                return -7003;
            }
        }
    }
}
