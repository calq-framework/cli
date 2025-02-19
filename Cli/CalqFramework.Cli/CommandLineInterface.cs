using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.DataAccess.InterfaceComponent;
using CalqFramework.Cli.Parsing;
using CalqFramework.Cli.Serialization;
using static CalqFramework.Cli.Parsing.OptionReaderBase;

namespace CalqFramework.Cli {

    public class CommandLineInterface {

        public CommandLineInterface() {
            CliOptionsStoreFactory = new CliComponentStoreFactory();
            HelpGenerator = new HelpPrinter();
        }

        public ICliComponentStoreFactory CliOptionsStoreFactory { get; init; }

        public bool SkipUnknown { get; init; } = false;
        public bool UseRevisionVersion { get; init; } = true;
        public IHelpPrinter HelpGenerator { get; init; }

        public object? Execute(object obj) {
            return Execute(obj, Environment.GetCommandLineArgs().Skip(1));
        }

        public object? Execute(object obj, IEnumerable<string> args) {
            using IEnumerator<string> en = args.GetEnumerator();
            if (!en.MoveNext()) {
                return null;
            }

            // enumerate args until en.Current is not a submodule
            ISubmoduleStore? previousSubmoduleStore;
            ISubmoduleStore? submoduleStore = null;
            object submodule = obj;
            string? previousArg;
            string? arg = null;
            do {
                previousSubmoduleStore = submoduleStore;
                submoduleStore = CliOptionsStoreFactory.CreateSubmoduleStore(submodule);
                previousArg = arg;
                arg = en.Current;
                if (submoduleStore.ContainsKey(arg)) {
                    submodule = submoduleStore[arg]!;
                } else {
                    break;
                }
            } while (en.MoveNext());

            string subcommandName = arg;

            if (subcommandName == "--version" || subcommandName == "-v") {
                return Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3);
            }

            ISubcommandStore subcommandStore = CliOptionsStoreFactory.CreateSubcommandStore(submodule);

            if (subcommandName == "--help" || subcommandName == "-h") {
                IOptionStore optionSore = CliOptionsStoreFactory.CreateOptionStore(submodule);
                bool isRoot = submodule == obj;
                if (isRoot) {
                    return HelpGenerator.PrintHelp(obj.GetType(), submoduleStore.GetSubmodules(), subcommandStore.GetSubcommands(CliOptionsStoreFactory.CreateSubcommandExecutor), optionSore.GetOptions());
                } else {
                    var submoduleComponent = previousSubmoduleStore!.GetSubmodules().Where(x => previousSubmoduleStore.ContainsKey(previousArg!)).First(); // use the store to check for the key to comply with case sensitivity
                    return HelpGenerator.PrintHelp(obj.GetType(), submoduleComponent, submoduleStore.GetSubmodules(), subcommandStore.GetSubcommands(CliOptionsStoreFactory.CreateSubcommandExecutor), optionSore.GetOptions());
                }
            }

            MethodInfo subcommand = subcommandStore[subcommandName];
            ISubcommandExecutorWithOptions subcommandExecutorWithOptions = CliOptionsStoreFactory.CreateSubcommandExecutorWithOptions(subcommand, submodule);

            bool hasArguments = en.MoveNext();
            if (hasArguments) {
                string firstArg = en.Current;

                if (firstArg == "--help" || firstArg == "-h") {
                    return HelpGenerator.PrintSubcommandHelp(obj.GetType(), subcommandStore.GetSubcommands(CliOptionsStoreFactory.CreateSubcommandExecutor).Where(x => x.MethodInfo == subcommand).First(), subcommandExecutorWithOptions.GetOptions());
                }

                IEnumerator<string> skippedEn = GetSkippedEnumerator(en).GetEnumerator();
                ReadParametersAndOptions(skippedEn, subcommandExecutorWithOptions);
            }

            return subcommandExecutorWithOptions.Invoke();
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
                if (optionAttr.HasFlag(OptionFlags.ValueUnassigned) && !optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                    if (SkipUnknown) {
                        continue;
                    }
                    throw new CliException($"unknown option {option}");
                }

                if (optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                    subcommandExecutorWithOptions.AddArgument(option);
                } else {
                    subcommandExecutorWithOptions[option] = value;
                }
            }

            while (args.MoveNext()) {
                subcommandExecutorWithOptions.AddArgument(args.Current);
            }
        }
    }
}
