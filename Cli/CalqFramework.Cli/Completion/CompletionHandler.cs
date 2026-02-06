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

        public ResultVoid HandleComplete(ICliContext context, IEnumerable<string> args, object target) {
            var argsList = args.ToList();
            
            if (argsList.Count == 0) {
                HandleComplete(context, target, new List<string>(), "");
                return ResultVoid.Value;
            }
            
            string toComplete = argsList[^1];
            var argsBeforeCursor = argsList.Take(argsList.Count - 1).ToList();
            
            HandleComplete(context, target, argsBeforeCursor, toComplete);
            return ResultVoid.Value;
        }

        public ResultVoid HandleCompletion(ICliContext context, IEnumerable<string> args, object target) {
            var argsList = args.ToList();
            
            if (argsList.Count == 0) {
                throw CliErrors.CompletionRequiresShell();
            }

            string shell = argsList[0];
            
            if (!CompletionScriptGenerator.SupportedShells.Contains(shell.ToLowerInvariant())) {
                throw CliErrors.UnsupportedShell(shell);
            }

            var remainingArgs = argsList.Skip(1).ToList();
            
            if (remainingArgs.Count > 0) {
                string action = remainingArgs[0];

                switch (action.ToLowerInvariant()) {
                    case "install":
                        HandleCompletionInstall(shell);
                        return ResultVoid.Value;
                    
                    case "uninstall":
                        HandleCompletionUninstall(shell);
                        return ResultVoid.Value;
                    
                    default:
                        throw CliErrors.UnknownCompletionAction(action);
                }
            }
            
            HandleCompletionScript(shell);
            return ResultVoid.Value;
        }

        private void HandleComplete(ICliContext context, object target, List<string> args, string toComplete) {
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
                
                bool hasMatches = submodules.Any(s => s.Keys.Any(k => k.StartsWith(toComplete, StringComparison.OrdinalIgnoreCase))) ||
                                  subcommands.Any(s => s.Keys.Any(k => k.StartsWith(toComplete, StringComparison.OrdinalIgnoreCase)));
                
                string effectivePartialInput = hasMatches ? toComplete : "";
                
                CompletionPrinter.PrintSubmodules(submodules, effectivePartialInput);
                CompletionPrinter.PrintSubcommands(subcommands, effectivePartialInput);
                return;
            }

            if (!subcommandStore.ContainsKey(subcommandName)) {
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

            string? previousArg = args.Count > 0 ? args[^1] : null;
            bool completingOptionValue = previousArg != null && previousArg.StartsWith("-") && !toComplete.StartsWith("-");
            
            if (toComplete.StartsWith("-")) {
                var parameters = subcommandExecutorWithOptions.GetParameters();
                var options = subcommandExecutorWithOptions.GetOptions();
                CompletionPrinter.PrintParametersAndOptions(parameters, options, toComplete);
            } else if (completingOptionValue && previousArg != null) {
                string optionName = previousArg.TrimStart('-', '+');
                
                var parameter = subcommandExecutorWithOptions.GetParameters().FirstOrDefault(p => p.Keys.Contains(optionName));
                if (parameter != null) {
                    CompletionPrinter.PrintParameterValue(parameter, toComplete, submodule);
                } else {
                    var option = subcommandExecutorWithOptions.GetOptions().FirstOrDefault(o => o.Keys.Contains(optionName));
                    if (option != null) {
                        CompletionPrinter.PrintOptionValue(option, toComplete, submodule);
                    }
                }
            } else {
                var parameter = subcommandExecutorWithOptions.GetFirstUnassignedParameter();
                
                if (parameter != null) {
                    CompletionPrinter.PrintParameterValue(parameter, toComplete, submodule);
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

        private void HandleCompletionScript(string shell) {
            var programName = Assembly.GetEntryAssembly()?.GetToolCommandName() ?? throw CliErrors.UnableToDetermineProgramName();
            var script = CompletionScriptGenerator.GenerateScript(shell, programName);
            Console.WriteLine(script);
        }

        private void HandleCompletionInstall(string shell) {
            var programName = Assembly.GetEntryAssembly()?.GetToolCommandName() ?? throw CliErrors.UnableToDetermineProgramName();

            try {
                CompletionScriptGenerator.InstallScript(shell, programName);
                var installPath = CompletionScriptGenerator.GetInstallPath(shell, programName);
                Console.WriteLine($"Completion script installed to: {installPath}");
                Console.WriteLine("Please restart your shell for changes to take effect.");
            } catch (Exception ex) {
                throw CliErrors.CompletionInstallFailed(shell, ex.Message, ex);
            }
        }

        private void HandleCompletionUninstall(string shell) {
            var programName = Assembly.GetEntryAssembly()?.GetToolCommandName() ?? throw CliErrors.UnableToDetermineProgramName();

            try {
                var removed = CompletionScriptGenerator.UninstallScript(shell, programName);
                
                if (removed) {
                    Console.WriteLine($"Completion script for {shell} has been uninstalled.");
                } else {
                    Console.WriteLine($"No completion script found for {shell}.");
                }
            } catch (Exception ex) {
                throw CliErrors.CompletionUninstallFailed(shell, ex.Message, ex);
            }
        }
    }
}
