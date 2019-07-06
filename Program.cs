namespace PowerShellWindowHost
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;

    public static class Program
    {
        private const string DoubleQuote = "\"";

        public static int Main()
        {
            ////Debugger.Launch();

            var arguments = GetArguments();

            GetAssemblyData(out var executingAssemblyLocation, out var executingAssemblyFileName);

            Configure(executingAssemblyFileName);

            SimpleLog.Info($"Full Commandline: {Environment.CommandLine}");
            SimpleLog.Info($"Detected Attributes: {arguments}");

            SimpleLog.Info($"RunHiddenConsole Executing Assembly FullName: {executingAssemblyFileName}");

            var targetExecutablePath = GetTargetExecutablePath(executingAssemblyLocation, executingAssemblyFileName);
            if (targetExecutablePath == null)
            {
                SimpleLog.Error("Unable to find target executable name in own executable name.");
                return -7001;
            }

            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = Directory.GetCurrentDirectory(),
                Arguments = arguments,
                FileName = targetExecutablePath,
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

        private static void Configure(string executingAssemblyFileName)
        {
            var logLevelString = ConfigurationManager.AppSettings["LogLevel"];

            if (logLevelString != null 
                && Enum.TryParse(logLevelString, true, out SimpleLog.Severity logLevel))
            {
                SimpleLog.LogLevel = logLevel;
            }
            else 
            {
                SimpleLog.LogLevel = SimpleLog.Severity.Error;
            }

            SimpleLog.BackgroundTaskDisabled = true;
            SimpleLog.Prefix = $"{executingAssemblyFileName}.";
        }

        private static void GetAssemblyData(out string assemblyLocation, out string assemblyFileName)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            assemblyFileName = Path.GetFileName(executingAssembly.Location);
            assemblyLocation = Path.GetDirectoryName(executingAssembly.Location) ?? string.Empty;
        }

        private static string GetTargetExecutablePath(string executingAssemblyLocation, string executingAssemblyFileName)
        {
            var match = Regex.Match(executingAssemblyFileName, @"(.+)w(\.\w{1,3})");
            if (!match.Success)
            {
                return null;
            }

            var targetExecutableName = match.Groups[1].Value + match.Groups[2].Value;
            var targetExecutablePath = Path.Combine(executingAssemblyLocation, targetExecutableName);

            return targetExecutablePath;
        }

        private static string GetArguments()
        {
            var commandLineExecutable = Environment
                .GetCommandLineArgs()[0]
                .Trim();
            
            var commandLine = Environment
                .CommandLine
                .Trim();

            var argsStartIndex = commandLineExecutable.Length 
                + (commandLine.StartsWith(DoubleQuote) 
                    ? 2 
                    : 0);

            var args = commandLine
                .Substring(argsStartIndex)
                .Trim();

            return args;
        }
    }
}
