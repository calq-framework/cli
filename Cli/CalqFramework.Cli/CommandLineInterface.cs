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
            CliComponentStoreFactory = new CliComponentStoreFactory();
            HelpPrinter = new HelpPrinter();
        }

        public ICliComponentStoreFactory CliComponentStoreFactory { get; init; }

        public IHelpPrinter HelpPrinter { get; init; }
        public bool SkipUnknown { get; init; } = false;
        public bool UseRevisionVersion { get; init; } = false;
        public object? Execute(object obj) {
            return Execute(obj, Environment.GetCommandLineArgs().Skip(1));
        }

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

            var result = subcommandExecutorWithOptions.Invoke();
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
                    subcommandExecutorWithOptions[option] = value;
                }
            }

            while (args.MoveNext()) {
                subcommandExecutorWithOptions.AddArgument(args.Current);
            }
        }
    }
}
