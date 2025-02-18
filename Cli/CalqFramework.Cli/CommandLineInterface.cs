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
            HelpGenerator = new HelpGenerator();
        }

        public ICliComponentStoreFactory CliOptionsStoreFactory { get; init; }

        public bool SkipUnknown { get; init; } = false;
        public bool UseRevisionVersion { get; init; } = true;
        private HelpGenerator HelpGenerator { get; init; } // TODO public

        public object? Execute(object obj) {
            return Execute(obj, Environment.GetCommandLineArgs().Skip(1));
        }

        public object? Execute(object obj, IEnumerable<string> args) {
            using IEnumerator<string> en = args.GetEnumerator();
            if (!en.MoveNext()) {
                return null;
            }

            // enumerate args until en.Current is not a submodule
            ISubmoduleStore submoduleStore;
            object submodule = obj;
            string arg;
            do {
                submoduleStore = CliOptionsStoreFactory.CreateSubmoduleStore(submodule);
                arg = en.Current;
                if (submoduleStore.ContainsKey(arg)) {
                    submodule = submoduleStore[arg]!;
                } else {
                    break;
                }
            } while (en.MoveNext());

            var subcommandName = arg;

            if (subcommandName == "--version" || subcommandName == "-v") {
                return Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3);
            }

            ISubcommandStore subcommandStore = CliOptionsStoreFactory.CreateSubcommandStore(submodule);

            if (subcommandName == "--help" || subcommandName == "-h") {
                IOptionStore optionSore = CliOptionsStoreFactory.CreateOptionStore(submodule);
                return HelpGenerator.GetHelp(optionSore.GetOptions(), submoduleStore.GetSubmodules(), subcommandStore.GetSubcommands(CliOptionsStoreFactory.CreateSubcommandExecutor));
            }

            MethodInfo subcommand = subcommandStore[subcommandName];
            var subcommandExecutorWithOptions = CliOptionsStoreFactory.CreateSubcommandExecutorWithOptions(subcommand, submodule);

            var hasArguments = en.MoveNext();
            if (hasArguments) {
                var firstArg = en.Current;

                if (firstArg == "--help" || firstArg == "-h") {
                    return HelpGenerator.GetHelp(subcommandExecutorWithOptions.GetOptions(), subcommandStore.GetSubcommands(CliOptionsStoreFactory.CreateSubcommandExecutor).Where(x => x.MethodInfo == subcommand).First());
                }

                var skippedEn = GetSkippedEnumerator(en).GetEnumerator();
                ReadParametersAndOptions(skippedEn, subcommandExecutorWithOptions, subcommand, subcommandStore);
            }

            return subcommandExecutorWithOptions.Invoke();
        }

        // TODO move as extension method or convert enumerator to enumerable
        private IEnumerable<string> GetSkippedEnumerator(IEnumerator<string> en) {
            do {
                yield return en.Current;
            } while (en.MoveNext());
        }

        private void ReadParametersAndOptions(IEnumerator<string> args, ISubcommandExecutorWithOptions subcommandExecutorWithOptions, MethodInfo subcommand, ISubcommandStore subcommandStore) {
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
