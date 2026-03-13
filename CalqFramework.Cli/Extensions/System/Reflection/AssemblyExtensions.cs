namespace CalqFramework.Cli.Extensions.System.Reflection;

public static class AssemblyExtensions {
    /// <summary>
    ///     Gets the tool command name for a .NET global tool.
    ///     First tries to find the actual tool command name using 'dotnet tool list --global',
    ///     then falls back to the assembly name.
    /// </summary>
    public static string GetToolCommandName(this Assembly assembly) {
        string? assemblyName = assembly?.GetName()
            .Name;

        if (string.IsNullOrEmpty(assemblyName)) {
            throw new InvalidOperationException("Unable to determine assembly name");
        }

        try {
            ProcessStartInfo processStartInfo = new() {
                FileName = "dotnet",
                Arguments = "tool list --global",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process? process = Process.Start(processStartInfo);
            if (process != null) {
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0) {
                    string[] lines = output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
                    foreach (string line in lines) {
                        if (line.Contains("Package Id") || line.Contains("---")) {
                            continue;
                        }

                        string[] parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 3) {
                            string packageId = parts[0];
                            string commandName = parts[2];

                            if (packageId.Equals(assemblyName, StringComparison.OrdinalIgnoreCase)) {
                                return commandName;
                            }
                        }
                    }
                }
            }
        } catch {
        }

        return assemblyName;
    }
}
