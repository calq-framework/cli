using System;
using System.Linq;
using System.Reflection;
using static CalqFramework.Cli.Parsing.OptionReaderBase;
using System.Collections.Generic;
using CalqFramework.Cli.Serialization;
using CalqFramework.Cli.DataAccess.ClassMember;
using System.ComponentModel.Design;
using CalqFramework.Cli.InterfaceComponents;
using CalqFramework.Cli.DataAccess.InterfaceComponent;
using CalqFramework.Cli.Parsing;

namespace CalqFramework.Cli {
    // TODO create separate class for help/version logic
    public class CommandLineInterface
    {
        public ICliComponentStoreFactory CliOptionsStoreFactory { get; init; }

        private HelpGenerator HelpGenerator { get; init; } // TODO public

        public bool SkipUnknown { get; init; } = false;

        public bool UseRevisionVersion { get; init; } = true;

        public CommandLineInterface() {
            CliOptionsStoreFactory = new CliComponentStoreFactory();
            HelpGenerator = new HelpGenerator();
        }

        // TODO separate data and printing
        private void HandleMethodHelp(OptionAndParameterStore optionsAndParams, MethodInfo methodInfo)
        {
            var method = new Subcommand() {
                // FIXME generics
                ReturnType = methodInfo.ReturnType,
                Keys = new[] { methodInfo.Name },
                MethodInfo = methodInfo,
                Parameters = new ParameterStore<string, object?, ParameterInfo>(new CliMethodParameterStore(methodInfo)).GetParameters()
            };
            Console.Write(HelpGenerator.GetHelp(optionsAndParams.Options.GetOptions(), method));
        }

        // TODO separate data and printing
        private void HandleInstanceHelp(IOptionStore<string, object?, MemberInfo> options, ISubmoduleStore<string, object?, MemberInfo> commands, MethodResolver methodResolver)
        {
            var methods = new List<Subcommand>();
            foreach (var methodInfo in methodResolver.Methods) {
                methods.Add(new Subcommand() {
                    // FIXME generics
                    ReturnType = methodInfo.ReturnType,
                    Keys = new[] { methodInfo.Name },
                    MethodInfo = methodInfo,
                    Parameters = new ParameterStore<string, object?, ParameterInfo>(new CliMethodParameterStore(methodInfo)).GetParameters()
                });
            }
            Console.Write(HelpGenerator.GetHelp(options.GetOptions(), commands.GetCommands(), methods));
        }

        private bool TryReadOptionsAndActionParams(IEnumerator<string> args, OptionAndParameterStore optionsAndParams, MethodInfo method)
        {
            var optionsReader = new OptionReader(args, optionsAndParams);

            try
            {
                foreach (var (option, value, optionAttr) in optionsReader.Read())
                {
                    if ((option == "help" || option == "h") && !optionAttr.HasFlag(OptionFlags.NotAnOption))
                    {
                        HandleMethodHelp(optionsAndParams, method);
                        return false;
                    }

                    if (optionAttr.HasFlag(OptionFlags.ValueUnassigned) && !optionAttr.HasFlag(OptionFlags.NotAnOption))
                    {
                        if (SkipUnknown) {
                            continue;
                        }
                        throw new CliException($"unknown option {option}");
                    }

                    if (optionAttr.HasFlag(OptionFlags.NotAnOption))
                    {
                        optionsAndParams._parameters.AddPositionalParameter(option);
                    }
                    else
                    {
                        var type = optionsAndParams.GetDataType(option);
                        var valueObj = ValueParser.ParseValue(value, type, option);
                        optionsAndParams[option] = valueObj;
                    }
                }
                while (args.MoveNext()) {
                    optionsAndParams._parameters.AddPositionalParameter(args.Current);
                }
            }
            catch (Exception ex)
            { // TODO rename the exception in CalqFramework.DataAccess
                if (ex.Message == "collision")
                {
                    throw new CliException(ex.Message, ex);
                }
                throw;
            }

            return true;
        }

        public object? Execute(object obj) {
            return Execute(obj, Environment.GetCommandLineArgs().Skip(1));
        }

        public object? Execute(object obj, IEnumerable<string> args)
        {
            using var en = args.GetEnumerator();
            if (!en.MoveNext()) {
                return null;
            }
            string optionOrAction;

            var targetObj = obj;
            ISubmoduleStore<string, object?, MemberInfo> cliCommands;

            // explore object tree until optionOrAction definitely cannot be an action (object not found by name)
            do {
                optionOrAction = en.Current;
                cliCommands = CliOptionsStoreFactory.CreateSubmoduleStore(targetObj);
                if (cliCommands.ContainsKey(optionOrAction)) {
                    var type = cliCommands.GetDataType(optionOrAction);
                    if (ValueParser.IsParseable(type)) {
                        break;
                    }
                    targetObj = cliCommands[optionOrAction]!;
                } else {
                    break;
                }
            } while (en.MoveNext());
            var cliOptions = CliOptionsStoreFactory.CreateOptionStore(targetObj);

            var methodResolver = CliOptionsStoreFactory.CreateMethodResolver(targetObj);
            if (optionOrAction == "--help" || optionOrAction == "-h")
            {
                HandleInstanceHelp(cliOptions, cliCommands, methodResolver);
                return null;
            }
            if (optionOrAction == "--version" || optionOrAction == "-v") {
                Console.WriteLine(Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3));
                return null;
            }

            var cliAction = methodResolver.GetMethod(optionOrAction);
            var actionParams = new CliMethodParameterStore(cliAction);
            var optionsAndActionParams = new OptionAndParameterStore(cliOptions, actionParams, targetObj);
            if (TryReadOptionsAndActionParams(en, optionsAndActionParams, cliAction))
            {
                return optionsAndActionParams.Invoke();
            }
            else
            {
                return null;
            }

            throw new CliException($"no command specified");
        }
    }
}
