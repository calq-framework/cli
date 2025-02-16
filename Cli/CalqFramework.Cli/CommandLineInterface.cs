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
            string optionOrAction;

            object targetObj = obj;
            ISubmoduleStore cliCommands;

            // explore object tree until optionOrAction definitely cannot be an action (object not found by name)
            do {
                optionOrAction = en.Current;
                cliCommands = CliOptionsStoreFactory.CreateSubmoduleStore(targetObj);
                if (cliCommands.ContainsKey(optionOrAction)) {
                    Type type = cliCommands.GetDataType(optionOrAction);
                    if (ValueParser.IsParseable(type)) {
                        break;
                    }
                    targetObj = cliCommands[optionOrAction]!;
                } else {
                    break;
                }
            } while (en.MoveNext());
            IOptionStore cliOptions = CliOptionsStoreFactory.CreateOptionStore(targetObj);

            ISubcommandStore methodResolver = CliOptionsStoreFactory.CreateSubcommandStore(targetObj);
            if (optionOrAction == "--help" || optionOrAction == "-h") {
                Console.Write(HelpGenerator.GetHelp(cliOptions.GetOptions(), cliCommands.GetSubmodules(), methodResolver.GetSubcommands(CliOptionsStoreFactory.CreateSubcommandExecutor)));
                return null;
            }
            if (optionOrAction == "--version" || optionOrAction == "-v") {
                Console.WriteLine(Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3));
                return null;
            }

            MethodInfo? cliAction = methodResolver[optionOrAction];
            ISubcommandExecutorWithOptions optionsAndActionParams = CliOptionsStoreFactory.CreateSubcommandExecutorWithOptions(cliAction, targetObj);
            if (TryReadOptionsAndActionParams(en, optionsAndActionParams, cliAction, methodResolver)) {
                return optionsAndActionParams.Invoke();
            } else {
                return null;
            }

            throw new CliException($"no command specified");
        }

        private bool TryReadOptionsAndActionParams(IEnumerator<string> args, ISubcommandExecutorWithOptions optionsAndParams, MethodInfo method, ISubcommandStore subcommands) {
            var optionsReader = new OptionReader(args, optionsAndParams);

            try {
                foreach ((string option, string value, OptionFlags optionAttr) in optionsReader.Read()) {
                    if ((option == "help" || option == "h") && !optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                        Console.Write(HelpGenerator.GetHelp(optionsAndParams.GetOptions(), subcommands.GetSubcommands(CliOptionsStoreFactory.CreateSubcommandExecutor).Where(x => x.MethodInfo == method).First()));
                        return false;
                    }

                    if (optionAttr.HasFlag(OptionFlags.ValueUnassigned) && !optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                        if (SkipUnknown) {
                            continue;
                        }
                        throw new CliException($"unknown option {option}");
                    }

                    if (optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                        optionsAndParams.AddArgument(option);
                    } else {
                        optionsAndParams[option] = value;
                    }
                }
                while (args.MoveNext()) {
                    optionsAndParams.AddArgument(args.Current);
                }
            } catch (Exception ex) { // TODO rename the exception in CalqFramework.DataAccess
                if (ex.Message == "collision") {
                    throw new CliException(ex.Message, ex);
                }
                throw;
            }

            return true;
        }
    }
}
