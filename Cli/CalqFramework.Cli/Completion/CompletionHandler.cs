using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.DataAccess.InterfaceComponents;
using CalqFramework.Cli.Extensions.System.Reflection;
using CalqFramework.Cli.Parsing;
using static CalqFramework.Cli.Parsing.OptionReaderBase;

namespace CalqFramework.Cli.Completion {

    public class CompletionHandler : ICompletionHandler {

        private ICompletionPrinter? _completionPrinter;
        private ICompletionScriptGenerator? _completionScriptGenerator;
        
        public ICompletionPrinter CompletionPrinter { 
            get => _completionPrinter ??= new CompletionPrinter();
            init => _completionPrinter = value;
        }
        
        public ICompletionScriptGenerator CompletionScriptGenerator { 
            get => _completionScriptGenerator ??= new CompletionScriptGenerator();
            init => _completionScriptGenerator = value;
        }

        public ResultVoid Handle(ICliContext context, string subcommand, IEnumerable<string> args, object target) {
            var argsList = args.ToList();
            
            switch (subcommand) {
                case "complete":
                    var completionArgs = new CompletionArgs();
                    OptionDeserializer.Deserialize(completionArgs, argsList);
                    ExecuteCompletion(context, target, completionArgs);
                    return ResultVoid.Value;
                
                case "script":
                    var scriptArgs = new CompletionScriptArgs();
                    OptionDeserializer.Deserialize(scriptArgs, argsList);
                    HandleCompletionScript(scriptArgs);
                    return ResultVoid.Value;
                
                case "install":
                    var installArgs = new CompletionInstallArgs();
                    OptionDeserializer.Deserialize(installArgs, argsList);
                    HandleCompletionInstall(installArgs);
                    return ResultVoid.Value;
                
                case "uninstall":
                    var uninstallArgs = new CompletionUninstallArgs();
                    OptionDeserializer.Deserialize(uninstallArgs, argsList);
                    HandleCompletionUninstall(uninstallArgs);
                    return ResultVoid.Value;
                
                default:
                    throw CliErrors.UnknownCompletionSubcommand(subcommand);
            }
        }

        private void ExecuteCompletion(ICliContext context, object target, CompletionArgs completionArgs) {
            var words = completionArgs.Words.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int position = completionArgs.Position;
            
            // Position is 0-indexed into the words array
            // Partial input is the word at the cursor position
            string partialInput = position >= 0 && position < words.Length ? words[position] : "";
            
            // Args before cursor are all words before position, excluding the program name (index 0)
            var argsBeforeCursor = words.Skip(1).Take(Math.Max(0, position - 1));
            
            ExecuteCompletion(context, target, argsBeforeCursor, partialInput, completionArgs);
        }

        private void ExecuteCompletion(ICliContext context, object target, IEnumerable<string> args, string partialInput, CompletionArgs completionArgs) {
            using IEnumerator<string> en = args.GetEnumerator();

            object? parentSubmodule = null;
            object submodule = target;
            ISubmoduleStore submoduleStore = context.CliComponentStoreFactory.CreateSubmoduleStore(submodule);
            string? submoduleName = null;
            string? subcommandName = null;
            
            while (en.MoveNext()) {
                var arg = en.Current;
                if (submoduleStore.ContainsKey(arg)) {
                    submoduleName = arg;
                    parentSubmodule = submodule;
                    submodule = submoduleStore[submoduleName]!;
                    submoduleStore = context.CliComponentStoreFactory.CreateSubmoduleStore(submodule);
                } else {
                    subcommandName = arg;
                    break;
                }
            }

            ISubcommandStore subcommandStore = context.CliComponentStoreFactory.CreateSubcommandStore(submodule);

            if (subcommandName == null) {
                var submodules = submoduleStore.GetSubmodules().ToList();
                var subcommands = subcommandStore.GetSubcommands(context.CliComponentStoreFactory.CreateSubcommandExecutor).ToList();
                
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
                CompletionPrinter.PrintSubcommands(subcommandStore.GetSubcommands(context.CliComponentStoreFactory.CreateSubcommandExecutor), subcommandName);
                return;
            }

            MethodInfo subcommand = subcommandStore[subcommandName]!;
            ISubcommandExecutorWithOptions subcommandExecutorWithOptions = context.CliComponentStoreFactory.CreateSubcommandExecutorWithOptions(subcommand, submodule);

            bool hasArguments = en.MoveNext();
            if (hasArguments) {
                IEnumerator<string> skippedEn = GetSkippedEnumerator(en).GetEnumerator();
                ReadParametersAndOptionsForCompletion(context, skippedEn, subcommandExecutorWithOptions);
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

        private void ReadParametersAndOptionsForCompletion(ICliContext context, IEnumerator<string> args, ISubcommandExecutorWithOptions subcommandExecutorWithOptions) {
            var optionReader = new OptionReader(args, subcommandExecutorWithOptions);

            foreach ((string option, string value, OptionFlags optionAttr) in optionReader.Read()) {
                if (optionAttr.HasFlag(OptionFlags.Unknown) && context.SkipUnknown) {
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

        private static IEnumerable<string> GetSkippedEnumerator(IEnumerator<string> en) {
            do {
                yield return en.Current;
            } while (en.MoveNext());
        }

        private void HandleCompletionScript(CompletionScriptArgs args) {
            var programName = args.ProgramName ?? Assembly.GetEntryAssembly()?.GetToolCommandName() ?? throw CliErrors.UnableToDetermineProgramName();
            var script = CompletionScriptGenerator.GenerateScript(args.Shell, programName);
            Console.WriteLine(script);
        }

        private void HandleCompletionInstall(CompletionInstallArgs args) {
            var programName = args.ProgramName ?? Assembly.GetEntryAssembly()?.GetToolCommandName() ?? throw CliErrors.UnableToDetermineProgramName();

            try {
                CompletionScriptGenerator.InstallScript(args.Shell, programName);
                var installPath = CompletionScriptGenerator.GetInstallPath(args.Shell, programName);
                Console.WriteLine($"Completion script installed to: {installPath}");
                Console.WriteLine("Please restart your shell for changes to take effect.");
            } catch (Exception ex) {
                throw CliErrors.CompletionInstallFailed(args.Shell, ex.Message, ex);
            }
        }

        private void HandleCompletionUninstall(CompletionUninstallArgs args) {
            var programName = args.ProgramName ?? Assembly.GetEntryAssembly()?.GetToolCommandName() ?? throw CliErrors.UnableToDetermineProgramName();

            try {
                var removed = CompletionScriptGenerator.UninstallScript(args.Shell, programName);
                
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
