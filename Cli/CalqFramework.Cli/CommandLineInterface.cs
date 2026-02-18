using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.DataAccess;
using CalqFramework.Cli.DataAccess.InterfaceComponentStores;
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
    public class CommandLineInterface : ICliContext {

        private ICompletionHandler? _completionHandler;
        private IDotnetSuggestHandler? _dotnetSuggestHandler;

        public CommandLineInterface() {
            CliComponentStoreFactory = new CliComponentStoreFactory();
            HelpPrinter = new HelpPrinter();
        }

        /// <summary>
        /// Factory for creating CLI component stores (options, subcommands, submodules).
        /// </summary>
        public ICliComponentStoreFactory CliComponentStoreFactory { get; init; }

        /// <summary>
        /// Help printer for displaying CLI help information.
        /// </summary>
        public IHelpPrinter HelpPrinter { get; init; }
        
        public ICompletionHandler CompletionHandler { 
            get => _completionHandler ??= new CompletionHandler();
            init => _completionHandler = value;
        }

        public IDotnetSuggestHandler DotnetSuggestHandler {
            get => _dotnetSuggestHandler ??= new DotnetSuggestHandler();
            init => _dotnetSuggestHandler = value;
        }
        
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
        public object? Execute(object target) {
            return Execute(target, Environment.GetCommandLineArgs().Skip(1));
        }

        /// <summary>
        /// Executes a CLI command using the provided arguments.
        /// </summary>
        public object? Execute(object target, IEnumerable<string> args) {
            var argsList = args.ToList();
            
            // Check if this is the dotnet-suggest protocol ([suggest] or [suggest:N] where N is cursor position)
            if (argsList.Count > 0 && argsList[0].StartsWith("[suggest")) {
                return DotnetSuggestHandler.HandleDotnetSuggest(this, CompletionHandler, argsList, target);
            }
            
            // Check if this is the __complete command (Cobra-style completion)
            if (argsList.Count > 0 && argsList[0] == "__complete") {
                return CompletionHandler.HandleComplete(this, argsList.Skip(1), target);
            }
            
            // Check if this is a completion management command
            if (argsList.Count > 0 && argsList[0] == "completion") {
                return CompletionHandler.HandleCompletion(this, argsList.Skip(1), target);
            }
            
            return ExecuteInvoke(target, argsList);
        }

        /// <summary>
        /// Executes a CLI command and invokes the subcommand.
        /// </summary>
        private object? ExecuteInvoke(object target, IEnumerable<string> args) {
            using IEnumerator<string> en = args.GetEnumerator();

            object? parentSubmodule = null;
            object submodule = target;
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
                bool isRoot = submodule == target;
                if (isRoot) {
                    HelpPrinter.PrintHelp(target.GetType(), submoduleStore.GetSubmodules(), subcommandStore.GetSubcommands(CliComponentStoreFactory.CreateSubcommandExecutor), optionStore.GetOptions());
                    return ResultVoid.Value;
                } else {
                    ISubmoduleStore parentSubmoduleStore = CliComponentStoreFactory.CreateSubmoduleStore(parentSubmodule!);
                    var submoduleInfo = parentSubmoduleStore.GetSubmodules().Where(x => parentSubmoduleStore[x.Keys.First()] == submodule).First(); // use the store to check for the key to comply with case sensitivity
                    HelpPrinter.PrintHelp(target.GetType(), submoduleInfo, submoduleStore.GetSubmodules(), subcommandStore.GetSubcommands(CliComponentStoreFactory.CreateSubcommandExecutor), optionStore.GetOptions());
                    return ResultVoid.Value;
                }
            }

            MethodInfo subcommand = subcommandStore[subcommandName!]!;
            ISubcommandExecutorWithOptions subcommandExecutorWithOptions = CliComponentStoreFactory.CreateSubcommandExecutorWithOptions(subcommand, submodule);

            bool hasArguments = en.MoveNext();
            if (hasArguments) {
                string firstArg = en.Current;

                if (firstArg == "--help" || firstArg == "-h") {
                    HelpPrinter.PrintSubcommandHelp(target.GetType(), subcommandStore.GetSubcommands(CliComponentStoreFactory.CreateSubcommandExecutor).Where(x => x.MethodInfo == subcommand).First(), subcommandExecutorWithOptions.GetOptions());
                    return ResultVoid.Value;
                }

                IEnumerator<string> skippedEn = GetSkippedEnumerator(en).GetEnumerator();
                ReadParametersAndOptions(skippedEn, subcommandExecutorWithOptions);
            }

            object? result;
            try {
                result = subcommandExecutorWithOptions.Invoke();
            } catch (DataAccessException ex) {
                throw CliErrors.FailedToAccessData(ex.Message, ex);
            }
            
            if (subcommand.ReturnType == typeof(void)) {
                return ResultVoid.Value;
            } else {
                return result;
            }
        }

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
                    } catch (DataAccessException ex) {
                        throw CliErrors.OptionValueError(option, value, ex.Message, ex);
                    }
                }
            }

            while (args.MoveNext()) {
                subcommandExecutorWithOptions.AddArgument(args.Current);
            }
        }
    }
}
