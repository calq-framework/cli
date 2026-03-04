using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CalqFramework.Cli.Completion;

/// <summary>
///     Handles dotnet-suggest protocol for shell completion integration.
/// </summary>
public sealed class DotnetSuggestHandler : IDotnetSuggestHandler {
    /// <summary>
    ///     Handles dotnet-suggest completion protocol.
    ///     Converts [suggest] or [suggest:N] format to __complete format and delegates to CompletionHandler.
    /// </summary>
    /// <param name="context">CLI context.</param>
    /// <param name="completionHandler">Completion handler to delegate to.</param>
    /// <param name="args">Arguments in dotnet-suggest format: [suggest] "command line" or [suggest:30] "command line".</param>
    /// <param name="target">Target instance.</param>
    public ResultVoid HandleDotnetSuggest(
        ICliContext context,
        ICompletionHandler completionHandler,
        IEnumerable<string> args,
        object target) {
        List<string> argsList = [.. args];

        if (argsList.Count < 2) {
            return ResultVoid.Value;
        }

        // Check for [suggest] or [suggest:N] directive
        string directive = argsList[0];
        if (!directive.StartsWith("[suggest") || !directive.EndsWith(']')) {
            return ResultVoid.Value;
        }

        // args[0] = "[suggest]" or "[suggest:30]"
        // args[1] = full command line as a single string (e.g., "cloudprovider deploy --v")

        // Extract cursor position if provided
        int? cursorPosition = null;
        if (directive.StartsWith("[suggest:")) {
            string posStr = directive.Substring(9, directive.Length - 10);
            if (int.TryParse(posStr, out int pos)) {
                cursorPosition = pos;
            }
        }

        // Get the full command line
        string fullCommandLine = argsList[1];
        string commandLine = fullCommandLine;

        // If cursor position is provided, only use the command line up to that position
        // This handles cases where cursor is in the middle: "deploy -- Normal" with cursor after "--"
        if (cursorPosition.HasValue && cursorPosition.Value < fullCommandLine.Length) {
            commandLine = fullCommandLine.Substring(0, cursorPosition.Value);
        }

        List<string> tokens = [.. commandLine.Split([' '], StringSplitOptions.None)];

        // dotnet-suggest includes the executable name in the command line
        // Skip it if present
        if (tokens.Count > 0) {
            string exeName = Path.GetFileNameWithoutExtension(Environment.ProcessPath ?? "");
            if (tokens[0].Equals(exeName, StringComparison.OrdinalIgnoreCase)) {
                tokens = [.. tokens.Skip(1)];
            }
        }

        // If cursor position is beyond the full command line length, we're completing after a space
        // Add an empty token to indicate we're starting a new argument
        if (cursorPosition.HasValue && cursorPosition.Value > fullCommandLine.Length) {
            tokens.Add(string.Empty);
        }

        return completionHandler.HandleComplete(context, tokens, target);
    }

    /// <summary>
    ///     Registers the current executable with dotnet-suggest.
    /// </summary>
    /// <param name="commandPath">Path to the command executable. If null, uses current process path.</param>
    public void Register(string? commandPath = null) {
        commandPath ??= Environment.ProcessPath;

        if (string.IsNullOrEmpty(commandPath)) {
            Console.Error.WriteLine("Unable to determine command path for registration.");
            return;
        }

        try {
            ProcessStartInfo startInfo = new() {
                FileName = "dotnet-suggest",
                Arguments = $"register --command-path \"{commandPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process? process = Process.Start(startInfo);
            if (process == null) {
                Console.Error.WriteLine("Failed to start dotnet-suggest registration.");
                return;
            }

            process.WaitForExit();

            if (process.ExitCode == 0) {
                Console.WriteLine($"Successfully registered with dotnet-suggest: {commandPath}");
            } else {
                string error = process.StandardError.ReadToEnd();
                Console.Error.WriteLine($"dotnet-suggest registration failed: {error}");
            }
        } catch (Exception ex) {
            Console.Error.WriteLine($"Error registering with dotnet-suggest: {ex.Message}");
            Console.WriteLine("\nTo register manually, run:");
            Console.WriteLine($"  dotnet-suggest register --command-path \"{commandPath}\"");
        }
    }

    /// <summary>
    ///     Unregisters the current executable from dotnet-suggest.
    /// </summary>
    /// <param name="commandPath">Path to the command executable. If null, uses current process path.</param>
    public void Unregister(string? commandPath = null) {
        commandPath ??= Environment.ProcessPath;

        if (string.IsNullOrEmpty(commandPath)) {
            Console.Error.WriteLine("Unable to determine command path for unregistration.");
            return;
        }

        try {
            ProcessStartInfo startInfo = new() {
                FileName = "dotnet-suggest",
                Arguments = $"unregister --command-path \"{commandPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process? process = Process.Start(startInfo);
            if (process == null) {
                Console.Error.WriteLine("Failed to start dotnet-suggest unregistration.");
                return;
            }

            process.WaitForExit();

            if (process.ExitCode == 0) {
                Console.WriteLine($"Successfully unregistered from dotnet-suggest: {commandPath}");
            } else {
                string error = process.StandardError.ReadToEnd();
                Console.Error.WriteLine($"dotnet-suggest unregistration failed: {error}");
            }
        } catch (Exception ex) {
            Console.Error.WriteLine($"Error unregistering from dotnet-suggest: {ex.Message}");
            Console.WriteLine("\nTo unregister manually, run:");
            Console.WriteLine($"  dotnet-suggest unregister --command-path \"{commandPath}\"");
        }
    }
}
