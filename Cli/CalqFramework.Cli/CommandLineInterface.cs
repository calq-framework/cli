using CalqFramework.Cli.DataAccess;
using System;
using System.Linq;
using System.Reflection;
using static CalqFramework.Cli.Parsing.OptionsReaderBase;
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
        private BindingFlags? _methodBindingAttr = null;

        public ICliOptionsStoreFactory CliOptionsStoreFactory { get; init; }

        private HelpGenerator HelpGenerator { get; init; } // TODO public

        public bool SkipUnknown { get; init; } = false;

        public BindingFlags MethodBindingAttr {
            get => _methodBindingAttr == null ? CliOptionsStoreFactory.BindingAttr : (BindingFlags)_methodBindingAttr;
            init => _methodBindingAttr = value;
        }

        public bool UseRevisionVersion { get; init; } = true;

        public CommandLineInterface() {
            CliOptionsStoreFactory = new CliOptionsStoreFactory();
            HelpGenerator = new HelpGenerator();
        }

        // TODO separate data and printing
        private void HandleMethodHelp(CliOptionsAndActionParametersStore optionsAndParams, MethodInfo methodInfo)
        {
            var method = new Method() {
                // FIXME generics
                ReturnType = methodInfo.ReturnType,
                Keys = new[] { methodInfo.Name },
                Methodinfo = methodInfo,
                PositionalParameters = new CliParamStore<string, object?, ParameterInfo>(new CliMethodParameterStore(methodInfo)).GetParamsString()
            };
            Console.Write(HelpGenerator.GetHelp(optionsAndParams.Options.GetOptionsString(), method));
        }

        // TODO separate data and printing
        private void HandleInstanceHelp(ICliOptionsStore<string, object?, MemberInfo> options, ICliCommandStore<string, object?, MemberInfo> commands, MethodResolver methodResolver)
        {
            var methods = new List<Method>();
            foreach (var methodInfo in methodResolver.Methods) {
                methods.Add(new Method() {
                    // FIXME generics
                    ReturnType = methodInfo.ReturnType,
                    Keys = new[] { methodInfo.Name },
                    Methodinfo = methodInfo,
                    PositionalParameters = new CliParamStore<string, object?, ParameterInfo>(new CliMethodParameterStore(methodInfo)).GetParamsString()
                });
            }
            Console.Write(HelpGenerator.GetHelp(options.GetOptionsString(), commands.GetCommandsString(), methods));
        }

        private bool TryReadOptionsAndActionParams(IEnumerator<string> args, CliOptionsAndActionParametersStore optionsAndParams, MethodInfo method)
        {
            var optionsReader = new OptionsReader(args, optionsAndParams);

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
                        optionsAndParams._actionParameters.AddPositionalParameter(option);
                    }
                    else
                    {
                        var type = optionsAndParams.GetDataType(option);
                        var valueObj = ValueParser.ParseValue(value, type, option);
                        optionsAndParams[option] = valueObj;
                    }
                }
                while (args.MoveNext()) {
                    optionsAndParams._actionParameters.AddPositionalParameter(args.Current);
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
            ICliCommandStore<string, object?, MemberInfo> cliCommands;

            // explore object tree until optionOrAction definitely cannot be an action (object not found by name)
            do {
                optionOrAction = en.Current;
                cliCommands = CliOptionsStoreFactory.CreateCommandStore(targetObj);
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
            var cliOptions = CliOptionsStoreFactory.CreateOptonStore(targetObj);

            var methodResolver = new MethodResolver(targetObj, MethodBindingAttr);
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
            var optionsAndActionParams = new CliOptionsAndActionParametersStore(cliOptions, actionParams, targetObj);
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
