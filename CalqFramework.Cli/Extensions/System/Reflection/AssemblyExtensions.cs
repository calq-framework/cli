using System.Reflection;

namespace CalqFramework.Cli.Extensions.System.Reflection {

    public static class AssemblyExtensions {

        /// <summary>
        /// Gets the tool command name for a .NET global tool.
        /// First tries to find the actual tool command name using 'dotnet tool list --global',
        /// then falls back to the assembly name.
        /// </summary>
        public static string GetToolCommandName(this Assembly assembly) {
            var assemblyName = assembly?.GetName().Name;
            
            if (string.IsNullOrEmpty(assemblyName)) {
                throw new global::System.InvalidOperationException("Unable to determine assembly name");
            }

            try {
                var processStartInfo = new global::System.Diagnostics.ProcessStartInfo {
                    FileName = "dotnet",
                    Arguments = "tool list --global",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = global::System.Diagnostics.Process.Start(processStartInfo);
                if (process != null) {
                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    if (process.ExitCode == 0) {
                        var lines = output.Split(new[] { '\r', '\n' }, global::System.StringSplitOptions.RemoveEmptyEntries);
                        foreach (var line in lines) {
                            if (line.Contains("Package Id") || line.Contains("---")) {
                                continue;
                            }

                            var parts = line.Split(new[] { ' ' }, global::System.StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length >= 3) {
                                var packageId = parts[0];
                                var commandName = parts[2];

                                if (packageId.Equals(assemblyName, global::System.StringComparison.OrdinalIgnoreCase)) {
                                    return commandName;
                                }
                            }
                        }
                    }
                }
            }
            catch {
            }

            return assemblyName;
        }
    }
}
