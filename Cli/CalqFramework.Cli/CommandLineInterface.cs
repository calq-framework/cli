using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.DataAccess.InterfaceComponent;
using CalqFramework.Cli.Parsing;
using CalqFramework.Cli.Serialization;
using static CalqFramework.Cli.Parsing.OptionReaderBase;

namespace CalqFramework.Cli {

    // TODO create separate class for help/version logic
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

            if (arg == "--version" || arg == "-v") {
                Console.WriteLine(Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3));
                return null;
            }

            ISubcommandStore subcommandStore = CliOptionsStoreFactory.CreateSubcommandStore(submodule);

            if (arg == "--help" || arg == "-h") {
                IOptionStore optionSore = CliOptionsStoreFactory.CreateOptionStore(submodule);
                Console.Write(HelpGenerator.GetHelp(optionSore.GetOptions(), submoduleStore.GetSubmodules(), subcommandStore.GetSubcommands(CliOptionsStoreFactory.CreateSubcommandExecutor)));
                return null;
            }

            MethodInfo subcommand = subcommandStore[arg];
            var subcommandExecutorWithOptions = CliOptionsStoreFactory.CreateSubcommandExecutorWithOptions(subcommand, submodule);
            if (TryReadOptionsAndActionParams(en, subcommandExecutorWithOptions, subcommand, subcommandStore)) {
                return subcommandExecutorWithOptions.Invoke();
            } else {
                return null;
            }

            throw new CliException($"no command specified");
        }

        private bool TryReadOptionsAndActionParams(IEnumerator<string> args, ISubcommandExecutorWithOptions subcommandExecutorWithOptions, MethodInfo subcommand, ISubcommandStore subcommandStore) {
            var optionReader = new OptionReader(args, subcommandExecutorWithOptions);

            foreach ((string option, string value, OptionFlags optionAttr) in optionReader.Read()) {
                if ((option == "help" || option == "h") && !optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                    Console.Write(HelpGenerator.GetHelp(subcommandExecutorWithOptions.GetOptions(), subcommandStore.GetSubcommands(CliOptionsStoreFactory.CreateSubcommandExecutor).Where(x => x.MethodInfo == subcommand).First()));
                    return false;
                }

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

            return true;
        }
    }
}
