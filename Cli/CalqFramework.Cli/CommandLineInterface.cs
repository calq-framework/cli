using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.DataAccess.InterfaceComponents;
using CalqFramework.Cli.Parsing;
using CalqFramework.Cli.Formatting;
using CalqFramework.Cli.Completion;
using CalqFramework.Cli.Extensions.System.Reflection;
using CalqFramework.DataAccess;
using static CalqFramework.Cli.Parsing.OptionReaderBase;

namespace CalqFramework.Cli {

    /// <summary>
    /// Interprets CLI commands and executes methods on any classlib without requiring programming.
    /// </summary>
    public class CommandLineInterface {

        public CommandLineInterface() {
            CliComponentStoreFactory = new CliComponentStoreFactory();
            HelpPrinter = new HelpPrinter();
            CompletionPrinter = new CompletionPrinter();
            CompletionScriptGenerator = new CompletionScriptGenerator();
        }

        /// <summary>
        /// Factory for creating CLI component stores (options, subcommands, submodules).
        /// </summary>
        public ICliComponentStoreFactory CliComponentStoreFactory { get; init; }

        /// <summary>
        /// Help printer for displaying CLI help information.
        /// </summary>
        public IHelpPrinter HelpPrinter { get; init; }
        
        /// <summary>
        /// Completion printer for displaying completion suggestions.
        /// </summary>
        public ICompletionPrinter CompletionPrinter { get; init; }
        
        /// <summary>
        /// Completion script generator for generating shell completion scripts.
        /// </summary>
        public ICompletionScriptGenerator CompletionScriptGenerator { get; init; }
        
        /// <summary>
        /// Skip unknown options instead of throwing an exception.
        /// </summary>
        public bool SkipUnknown { get; init; } = false;
        
        /// <summary>
        /// Include revision number in version output (4 digits instead of 3).
        /// </summary>
        public bool UseRevisionVersion { get; init; } = false;
        
        /// <summary>
        /// Executes a CLI command using command-line arguments from the environment.
        /// </summary>
        public object? Execute(object obj) {
            return Execute(obj, Environment.GetCommandLineArgs().Skip(1));
        }

        /// <summary>
        /// Executes a CLI command using the provided arguments.
        /// </summary>
        public object? Execute(object obj, IEnumerable<string> args) {
            var argsList = args.ToList();
            
            // Check if this is a completion-related command
            if (argsList.Count >= 2 && argsList[0] == "completion") {
                var subcommand = argsList[1];
                
                switch (subcommand) {
                    case "complete":
                        var completionArgs = new CompletionArgs();
                        OptionDeserializer.Deserialize(completionArgs, argsList.Skip(2));
                        ExecuteCompletion(obj, completionArgs);
                        return ResultVoid.Value;
                    
                    case "script":
                        var scriptArgs = new CompletionScriptArgs();
                        OptionDeserializer.Deserialize(scriptArgs, argsList.Skip(2));
                        ExecuteCompletionScript(scriptArgs);
                        return ResultVoid.Value;
                    
                    case "install":
                        var installArgs = new CompletionInstallArgs();
                        OptionDeserializer.Deserialize(installArgs, argsList.Skip(2));
                        ExecuteCompletionInstall(installArgs);
                        return ResultVoid.Value;
                    
                    case "uninstall":
                        var uninstallArgs = new CompletionUninstallArgs();
                        OptionDeserializer.Deserialize(uninstallArgs, argsList.Skip(2));
                        ExecuteCompletionUninstall(uninstallArgs);
                        return ResultVoid.Value;
                }
            }
            
            return ExecuteInvoke(obj, argsList);
        }

        /// <summary>
        /// Executes a CLI command and invokes the subcommand.
        /// </summary>
        private object? ExecuteInvoke(object obj, IEnumerable<string> args) {
            using IEnumerator<string> en = args.GetEnumerator();

            object? parentSubmodule = null;
            object submodule = obj;
            ISubmoduleStore submoduleStore = CliComponentStoreFactory.CreateSubmoduleStore(submodule);
            string? submoduleName = null;
            string? subcommandName = null;
            while (en.MoveNext()) {
                var arg = en.Current;
                if (submoduleStore.ContainsKey(arg)) {
                    submoduleName = arg;
                    parentSubmodule = submodule;
                    submodule = submoduleStore[submoduleName]!;
                    submoduleStore = CliComponentStoreFactory.CreateSubmoduleStore(submodule);
                } else {
                    subcommandName = arg;
                    break;
                }
            }

            if (subcommandName == "--version" || subcommandName == "-v") {
                return Assembly.GetEntryAssembly()?.GetName().Version?.ToString(UseRevisionVersion ? 4 : 3);
            }

            ISubcommandStore subcommandStore = CliComponentStoreFactory.CreateSubcommandStore(submodule);

            if (subcommandName == null || subcommandName == "--help" || subcommandName == "-h") {
                IOptionStore optionStore = CliComponentStoreFactory.CreateOptionStore(submodule);
                bool isRoot = submodule == obj;
                if (isRoot) {
                    HelpPrinter.PrintHelp(obj.GetType(), submoduleStore.GetSubmodules(), subcommandStore.GetSubcommands(CliComponentStoreFactory.CreateSubcommandExecutor), optionStore.GetOptions());
                    return ResultVoid.Value;
                } else {
                    ISubmoduleStore parentSubmoduleStore = CliComponentStoreFactory.CreateSubmoduleStore(parentSubmodule!);
                    var submoduleInfo = parentSubmoduleStore.GetSubmodules().Where(x => parentSubmoduleStore[x.Keys.First()] == submodule).First(); // use the store to check for the key to comply with case sensitivity
                    HelpPrinter.PrintHelp(obj.GetType(), submoduleInfo, submoduleStore.GetSubmodules(), subcommandStore.GetSubcommands(CliComponentStoreFactory.CreateSubcommandExecutor), optionStore.GetOptions());
                    return ResultVoid.Value;
                }
            }

            MethodInfo subcommand = subcommandStore[subcommandName!]!;
            ISubcommandExecutorWithOptions subcommandExecutorWithOptions = CliComponentStoreFactory.CreateSubcommandExecutorWithOptions(subcommand, submodule);

            bool hasArguments = en.MoveNext();
            if (hasArguments) {
                string firstArg = en.Current;

                if (firstArg == "--help" || firstArg == "-h") {
                    HelpPrinter.PrintSubcommandHelp(obj.GetType(), subcommandStore.GetSubcommands(CliComponentStoreFactory.CreateSubcommandExecutor).Where(x => x.MethodInfo == subcommand).First(), subcommandExecutorWithOptions.GetOptions());
                    return ResultVoid.Value;
                }

                IEnumerator<string> skippedEn = GetSkippedEnumerator(en).GetEnumerator();
                ReadParametersAndOptions(skippedEn, subcommandExecutorWithOptions);
            }

            object? result;
            try {
                result = subcommandExecutorWithOptions.Invoke();
            } catch (ArgValueParserException ex) {
                throw CliErrors.FailedToParseArgument(ex.Message, ex);
            } catch (DataAccessException ex) {
                throw CliErrors.FailedToAccessData(ex.Message, ex);
            }
            
            if (subcommand.ReturnType == typeof(void)) {
                return ResultVoid.Value;
            } else {
                return result;
            }
        }

        /// <summary>
        /// Executes completion and prints completion suggestions.
        /// </summary>
        private void ExecuteCompletion(object obj, CompletionArgs completionArgs) {
            var words = completionArgs.Words.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int position = completionArgs.Position;
            
            // Position is 0-indexed into the words array
            // Partial input is the word at the cursor position
            string partialInput = position >= 0 && position < words.Length ? words[position] : "";
            
            // Args before cursor are all words before position, excluding the program name (index 0)
            var argsBeforeCursor = words.Skip(1).Take(Math.Max(0, position - 1));
            
            ExecuteCompletionInternal(obj, argsBeforeCursor, partialInput, completionArgs);
        }

        private void ExecuteCompletionInternal(object obj, IEnumerable<string> args, string partialInput, CompletionArgs completionArgs) {
            using IEnumerator<string> en = args.GetEnumerator();

            object? parentSubmodule = null;
            object submodule = obj;
            ISubmoduleStore submoduleStore = CliComponentStoreFactory.CreateSubmoduleStore(submodule);
            string? submoduleName = null;
            string? subcommandName = null;
            
            while (en.MoveNext()) {
                var arg = en.Current;
                if (submoduleStore.ContainsKey(arg)) {
                    submoduleName = arg;
                    parentSubmodule = submodule;
                    submodule = submoduleStore[submoduleName]!;
                    submoduleStore = CliComponentStoreFactory.CreateSubmoduleStore(submodule);
                } else {
                    subcommandName = arg;
                    break;
                }
            }

            ISubcommandStore subcommandStore = CliComponentStoreFactory.CreateSubcommandStore(submodule);

            if (subcommandName == null) {
                var submodules = submoduleStore.GetSubmodules().ToList();
                var subcommands = subcommandStore.GetSubcommands(CliComponentStoreFactory.CreateSubcommandExecutor).ToList();
                
                bool hasMatches = submodules.Any(s => s.Keys.Any(k => k.StartsWith(partialInput, StringComparison.OrdinalIgnoreCase))) ||
                                  subcommands.Any(s => s.Keys.Any(k => k.StartsWith(partialInput, StringComparison.OrdinalIgnoreCase)));
                
                // If no matches, show all options (use empty string as partial input)
                string effectivePartialInput = hasMatches ? partialInput : "";
                
                CompletionPrinter.PrintSubmodules(submodules, effectivePartialInput);
                CompletionPrinter.PrintSubcommands(subcommands, effectivePartialInput);
                return;
            }

            if (!subcommandStore.ContainsKey(subcommandName)) {
                // Use the invalid subcommand name as the partial input for matching
                CompletionPrinter.PrintSubcommands(subcommandStore.GetSubcommands(CliComponentStoreFactory.CreateSubcommandExecutor), subcommandName);
                return;
            }

            MethodInfo subcommand = subcommandStore[subcommandName]!;
            ISubcommandExecutorWithOptions subcommandExecutorWithOptions = CliComponentStoreFactory.CreateSubcommandExecutorWithOptions(subcommand, submodule);

            bool hasArguments = en.MoveNext();
            if (hasArguments) {
                IEnumerator<string> skippedEn = GetSkippedEnumerator(en).GetEnumerator();
                ReadParametersAndOptionsForCompletion(skippedEn, subcommandExecutorWithOptions);
            }

            // Check if we're completing an option/parameter value (previous word starts with -)
            var words = completionArgs.Words.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string? previousWord = completionArgs.Position > 0 && completionArgs.Position <= words.Length ? words[completionArgs.Position - 1] : null;
            bool completingOptionValue = previousWord != null && previousWord.StartsWith("-") && !partialInput.StartsWith("-");
            
            if (partialInput.StartsWith("-")) {
                var parameters = subcommandExecutorWithOptions.GetParameters();
                var options = subcommandExecutorWithOptions.GetOptions();
                CompletionPrinter.PrintParametersAndOptions(parameters, options, partialInput);
            } else if (completingOptionValue && previousWord != null) {
                string optionName = previousWord.TrimStart('-', '+');
                
                var parameter = subcommandExecutorWithOptions.GetParameters().FirstOrDefault(p => p.Keys.Contains(optionName));
                if (parameter != null) {
                    CompletionPrinter.PrintParameterValue(parameter, partialInput);
                } else {
                    // Check if it's an option (field/property)
                    var option = subcommandExecutorWithOptions.GetOptions().FirstOrDefault(o => o.Keys.Contains(optionName));
                    if (option != null) {
                        CompletionPrinter.PrintOptionValue(option, partialInput);
                    }
                }
            } else {
                var parameter = subcommandExecutorWithOptions.GetFirstUnassignedParameter();
                
                if (parameter != null) {
                    CompletionPrinter.PrintParameterValue(parameter, partialInput);
                }
            }
        }

        private void ReadParametersAndOptionsForCompletion(IEnumerator<string> args, ISubcommandExecutorWithOptions subcommandExecutorWithOptions) {
            var optionReader = new OptionReader(args, subcommandExecutorWithOptions);

            foreach ((string option, string value, OptionFlags optionAttr) in optionReader.Read()) {
                if (optionAttr.HasFlag(OptionFlags.Unknown) && SkipUnknown) {
                    continue;
                }

                if (optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                    subcommandExecutorWithOptions.AddArgument(option);
                } else {
                    try {
                        subcommandExecutorWithOptions[option] = value;
                    } catch {
                        // Ignore errors during completion parsing
                    }
                }
            }

            while (args.MoveNext()) {
                subcommandExecutorWithOptions.AddArgument(args.Current);
            }
        }

        // TODO move as extension method
        private static IEnumerable<string> GetSkippedEnumerator(IEnumerator<string> en) {
            do {
                yield return en.Current;
            } while (en.MoveNext());
        }

        private void ReadParametersAndOptions(IEnumerator<string> args, ISubcommandExecutorWithOptions subcommandExecutorWithOptions) {
            var optionReader = new OptionReader(args, subcommandExecutorWithOptions);

            foreach ((string option, string value, OptionFlags optionAttr) in optionReader.Read()) {
                if (optionAttr.HasFlag(OptionFlags.AmbigousValue)) {
                    throw CliErrors.AmbiguousValue(optionReader.ArgsEnumerator.Current, option);
                }
                if (optionAttr.HasFlag(OptionFlags.Unknown)) {
                    if (SkipUnknown) {
                        continue;
                    }
                    throw CliErrors.UnknownOption(option);
                }
                if (optionAttr.HasFlag(OptionFlags.ValueUnassigned) && !optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                    throw CliErrors.OptionRequiresValue(option);
                }

                if (optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                    subcommandExecutorWithOptions.AddArgument(option);
                } else {
                    try {
                        subcommandExecutorWithOptions[option] = value;
                    } catch (ArgValueParserException ex) {
                        throw CliErrors.OptionValueError(option, value, ex.Message, ex);
                    } catch (DataAccessException ex) {
                        throw CliErrors.OptionValueError(option, value, ex.Message, ex);
                    }
                }
            }

            while (args.MoveNext()) {
                subcommandExecutorWithOptions.AddArgument(args.Current);
            }
        }

        /// <summary>
        /// Executes the completion script command and outputs the script.
        /// </summary>
        private void ExecuteCompletionScript(CompletionScriptArgs args) {
            var programName = args.ProgramName ?? Assembly.GetEntryAssembly()?.GetToolCommandName() ?? throw CliErrors.UnableToDetermineProgramName();
            var script = args.Shell.ToLower() switch {
                "bash" => CompletionScriptGenerator.GenerateBashScript(programName),
                "zsh" => CompletionScriptGenerator.GenerateZshScript(programName),
                "powershell" or "pwsh" => CompletionScriptGenerator.GeneratePowerShellScript(programName),
                "fish" => CompletionScriptGenerator.GenerateFishScript(programName),
                _ => throw CliErrors.UnsupportedShell(args.Shell)
            };

            Console.WriteLine(script);
        }

        /// <summary>
        /// Executes the completion install command.
        /// </summary>
        private void ExecuteCompletionInstall(CompletionInstallArgs args) {
            var programName = args.ProgramName ?? Assembly.GetEntryAssembly()?.GetToolCommandName() ?? throw CliErrors.UnableToDetermineProgramName();
            var script = args.Shell.ToLower() switch {
                "bash" => CompletionScriptGenerator.GenerateBashScript(programName),
                "zsh" => CompletionScriptGenerator.GenerateZshScript(programName),
                "powershell" or "pwsh" => CompletionScriptGenerator.GeneratePowerShellScript(programName),
                "fish" => CompletionScriptGenerator.GenerateFishScript(programName),
                _ => throw CliErrors.UnsupportedShell(args.Shell)
            };

            try {
                CompletionScriptGenerator.InstallScript(args.Shell.ToLower(), programName, script);
                var installPath = CompletionScriptGenerator.GetInstallPath(args.Shell.ToLower(), programName);
                Console.WriteLine($"Completion script installed to: {installPath}");
                
                if (args.Shell.ToLower() is "bash" or "zsh") {
                    Console.WriteLine($"Please restart your shell or run: source {installPath}");
                } else if (args.Shell.ToLower() is "powershell" or "pwsh") {
                    Console.WriteLine("Please restart PowerShell for changes to take effect.");
                } else if (args.Shell.ToLower() == "fish") {
                    Console.WriteLine("Completion will be available in new Fish shell sessions.");
                }
            } catch (Exception ex) {
                throw CliErrors.CompletionInstallFailed(args.Shell, ex.Message, ex);
            }
        }

        /// <summary>
        /// Executes the completion uninstall command.
        /// </summary>
        private void ExecuteCompletionUninstall(CompletionUninstallArgs args) {
            var programName = args.ProgramName ?? Assembly.GetEntryAssembly()?.GetToolCommandName() ?? throw CliErrors.UnableToDetermineProgramName();

            try {
                var removed = CompletionScriptGenerator.UninstallScript(args.Shell.ToLower(), programName);
                
                if (removed) {
                    Console.WriteLine($"Completion script for {args.Shell} has been uninstalled.");
                } else {
                    Console.WriteLine($"No completion script found for {args.Shell}.");
                }
            } catch (Exception ex) {
                throw CliErrors.CompletionUninstallFailed(args.Shell, ex.Message, ex);
            }
        }
    }
}
