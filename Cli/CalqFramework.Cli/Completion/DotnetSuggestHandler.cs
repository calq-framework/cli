using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CalqFramework.Cli.Completion {

    /// <summary>
    /// Handles dotnet-suggest protocol for shell completion integration.
    /// </summary>
    public class DotnetSuggestHandler : IDotnetSuggestHandler {

        /// <summary>
        /// Handles dotnet-suggest completion protocol.
        /// Converts [suggest] or [suggest:N] format to __complete format and delegates to CompletionHandler.
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

            var argsList = args.ToList();

            if (argsList.Count < 2) {
                return ResultVoid.Value;
            }

            // Check for [suggest] or [suggest:N] directive
            var directive = argsList[0];
            if (!directive.StartsWith("[suggest") || !directive.EndsWith("]")) {
                return ResultVoid.Value;
            }

            // args[0] = "[suggest]" or "[suggest:30]"
            // args[1] = full command line as a single string (e.g., "cloudprovider deploy --v")
            
            // Extract cursor position if provided
            int? cursorPosition = null;
            if (directive.StartsWith("[suggest:")) {
                var posStr = directive.Substring(9, directive.Length - 10);
                if (int.TryParse(posStr, out var pos)) {
                    cursorPosition = pos;
                }
            }
            
            // Get the full command line
            var fullCommandLine = argsList[1];
            var commandLine = fullCommandLine;
            
            // If cursor position is provided, only use the command line up to that position
            // This handles cases where cursor is in the middle: "deploy -- Normal" with cursor after "--"
            if (cursorPosition.HasValue && cursorPosition.Value < fullCommandLine.Length) {
                commandLine = fullCommandLine.Substring(0, cursorPosition.Value);
            }
            
            var tokens = commandLine.Split(new[] { ' ' }, StringSplitOptions.None).ToList();

            // dotnet-suggest includes the executable name in the command line
            // Skip it if present
            if (tokens.Count > 0) {
                var exeName = System.IO.Path.GetFileNameWithoutExtension(Environment.ProcessPath ?? "");
                if (tokens[0].Equals(exeName, StringComparison.OrdinalIgnoreCase)) {
                    tokens = tokens.Skip(1).ToList();
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
        /// Registers the current executable with dotnet-suggest.
        /// </summary>
        /// <param name="commandPath">Path to the command executable. If null, uses current process path.</param>
        public void Register(string? commandPath = null) {
            commandPath ??= Environment.ProcessPath;

            if (string.IsNullOrEmpty(commandPath)) {
                Console.Error.WriteLine("Unable to determine command path for registration.");
                return;
            }

            try {
                var startInfo = new ProcessStartInfo {
                    FileName = "dotnet-suggest",
                    Arguments = $"register --command-path \"{commandPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(startInfo);
                if (process == null) {
                    Console.Error.WriteLine("Failed to start dotnet-suggest registration.");
                    return;
                }

                process.WaitForExit();

                if (process.ExitCode == 0) {
                    Console.WriteLine($"Successfully registered with dotnet-suggest: {commandPath}");
                } else {
                    var error = process.StandardError.ReadToEnd();
                    Console.Error.WriteLine($"dotnet-suggest registration failed: {error}");
                }
            } catch (Exception ex) {
                Console.Error.WriteLine($"Error registering with dotnet-suggest: {ex.Message}");
                Console.WriteLine($"\nTo register manually, run:");
                Console.WriteLine($"  dotnet-suggest register --command-path \"{commandPath}\"");
            }
        }

        /// <summary>
        /// Unregisters the current executable from dotnet-suggest.
        /// </summary>
        /// <param name="commandPath">Path to the command executable. If null, uses current process path.</param>
        public void Unregister(string? commandPath = null) {
            commandPath ??= Environment.ProcessPath;

            if (string.IsNullOrEmpty(commandPath)) {
                Console.Error.WriteLine("Unable to determine command path for unregistration.");
                return;
            }

            try {
                var startInfo = new ProcessStartInfo {
                    FileName = "dotnet-suggest",
                    Arguments = $"unregister --command-path \"{commandPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(startInfo);
                if (process == null) {
                    Console.Error.WriteLine("Failed to start dotnet-suggest unregistration.");
                    return;
                }

                process.WaitForExit();

                if (process.ExitCode == 0) {
                    Console.WriteLine($"Successfully unregistered from dotnet-suggest: {commandPath}");
                } else {
                    var error = process.StandardError.ReadToEnd();
                    Console.Error.WriteLine($"dotnet-suggest unregistration failed: {error}");
                }
            } catch (Exception ex) {
                Console.Error.WriteLine($"Error unregistering from dotnet-suggest: {ex.Message}");
                Console.WriteLine($"\nTo unregister manually, run:");
                Console.WriteLine($"  dotnet-suggest unregister --command-path \"{commandPath}\"");
            }
        }
    }
}
