using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.DataAccess.InterfaceComponents;
using CalqFramework.Cli.Parsing;
using CalqFramework.Cli.Formatting;
using static CalqFramework.Cli.Parsing.OptionReaderBase;

namespace CalqFramework.Cli {

    /// <summary>
    /// Interprets CLI commands and executes methods on any classlib without requiring programming.
    /// </summary>
    public class CommandLineInterface {

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
                IOptionStore optionSore = CliComponentStoreFactory.CreateOptionStore(submodule);
                bool isRoot = submodule == obj;
                if (isRoot) {
                    HelpPrinter.PrintHelp(obj.GetType(), submoduleStore.GetSubmodules(), subcommandStore.GetSubcommands(CliComponentStoreFactory.CreateSubcommandExecutor), optionSore.GetOptions());
                    return ResultVoid.Value;
                } else {
                    ISubmoduleStore parentSubmoduleStore = CliComponentStoreFactory.CreateSubmoduleStore(parentSubmodule!);
                    var submoduleInfo = parentSubmoduleStore.GetSubmodules().Where(x => parentSubmoduleStore[x.Keys.First()] == submodule).First(); // use the store to check for the key to comply with case sensitivity
                    HelpPrinter.PrintHelp(obj.GetType(), submoduleInfo, submoduleStore.GetSubmodules(), subcommandStore.GetSubcommands(CliComponentStoreFactory.CreateSubcommandExecutor), optionSore.GetOptions());
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
            } catch (CliValueParserException ex) {
                throw new CliException($"Failed to parse argument: {ex.Message}", ex);
            }
            
            if (subcommand.ReturnType == typeof(void)) {
                return ResultVoid.Value;
            } else {
                return result;
            }
        }

        // TODO move as extension method or convert enumerator to enumerable
        private static IEnumerable<string> GetSkippedEnumerator(IEnumerator<string> en) {
            do {
                yield return en.Current;
            } while (en.MoveNext());
        }

        private void ReadParametersAndOptions(IEnumerator<string> args, ISubcommandExecutorWithOptions subcommandExecutorWithOptions) {
            var optionReader = new OptionReader(args, subcommandExecutorWithOptions);

            foreach ((string option, string value, OptionFlags optionAttr) in optionReader.Read()) {
                if (optionAttr.HasFlag(OptionFlags.AmbigousValue)) {
                    throw new CliException($"Ambigious value {optionReader.ArgsEnumerator.Current} for {option}. Use option=value format for values starting with '-' or '+'.");
                }
                if (optionAttr.HasFlag(OptionFlags.ValueUnassigned) && !optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                    throw new CliException($"{option} requires a value");
                }
                if (optionAttr.HasFlag(OptionFlags.Unknown)) {
                    if (SkipUnknown) {
                        continue;
                    }
                    throw new CliException($"unknown option {option}");
                }

                if (optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                    subcommandExecutorWithOptions.AddArgument(option);
                } else {
                    try {
                        subcommandExecutorWithOptions[option] = value;
                    } catch (CliValueParserException ex) {
                        throw new CliException(option, value, ex.Message, ex);
                    }
                }
            }

            while (args.MoveNext()) {
                subcommandExecutorWithOptions.AddArgument(args.Current);
            }
        }
    }
}
