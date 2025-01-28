using CalqFramework.Cli.DataAccess;
using System;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.Serialization.Parsing;
using static CalqFramework.Cli.Serialization.Parsing.OptionsReaderBase;
using System.Collections.Generic;
using CalqFramework.DataAccess;
using CalqFramework.DataAccess.ClassMember;

namespace CalqFramework.Cli {
    // TODO create separate class for help/version logic
    public class CommandLineInterface
    {
        private BindingFlags? _methodBindingAttr = null;

        public CliOptionsStoreFactory CliOptionsStoreFactory { get; init; }

        public bool SkipUnknown { get; init; } = false;

        public BindingFlags MethodBindingAttr {
            get => _methodBindingAttr == null ? CliOptionsStoreFactory.BindingAttr : (BindingFlags)_methodBindingAttr;
            init => _methodBindingAttr = value;
        }

        public bool UseRevisionVersion { get; init; } = true;

        public CommandLineInterface() {
            CliOptionsStoreFactory ??= new CliOptionsStoreFactory();
        }

        // TODO separate data and printing
        private static void HandleMethodHelp(CliOptionsAndActionParametersStore optionsAndParams)
        {
            Console.Write(optionsAndParams.GetHelpString());
        }

        // TODO separate data and printing
        private static void HandleInstanceHelp(ICliOptionsStore options, MethodResolver methodResolver)
        {
            Console.WriteLine(options.CliSerializer.GetCommandsString());

            Console.WriteLine("[ACTION COMMANDS]");
            foreach (var methodInfo in methodResolver.Methods) {
                Console.WriteLine(methodResolver.MethodToString(methodInfo));
            }
            Console.WriteLine();

            Console.Write(options.CliSerializer.GetOptionsString());
        }

        private bool TryReadOptionsAndActionParams(IEnumerator<string> args, CliOptionsAndActionParametersStore optionsAndParams)
        {
            var optionsReader = new OptionsReader(args, optionsAndParams);

            var parameterIndex = 0;
            // TODO move this logic to the store
            void SetPositionalParameter(string option)
            {
                var parameterName = optionsAndParams.ActionParameters.GetKey(parameterIndex);
                var parameterType = optionsAndParams.ActionParameters.GetType(parameterIndex);
                optionsAndParams.ActionParameters.SetValue(parameterIndex, ValueParser.ParseValue(option, parameterType, parameterName));
                ++parameterIndex;
            }

            try
            {
                foreach (var (option, value, optionAttr) in optionsReader.Read())
                {
                    if ((option == "help" || option == "h") && !optionAttr.HasFlag(OptionFlags.NotAnOption))
                    {
                        HandleMethodHelp(optionsAndParams);
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
                        SetPositionalParameter(option);
                    }
                    else
                    {
                        var type = optionsAndParams.GetDataType(option);
                        var valueObj = ValueParser.ParseValue(value, type, option);
                        optionsAndParams[option] = valueObj;
                    }
                }
                while (args.MoveNext()) {
                    SetPositionalParameter(args.Current);
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
            ICliOptionsStore cliOptions;

            // explore object tree until optionOrAction definitely cannot be an action (object not found by name)
            do {
                optionOrAction = en.Current;
                cliOptions = CliOptionsStoreFactory.CreateCliStore(targetObj);
                if (cliOptions.ContainsKey(optionOrAction)) {
                    var type = cliOptions.GetDataType(optionOrAction);
                    if (ValueParser.IsParseable(type)) {
                        break;
                    }
                    targetObj = cliOptions[optionOrAction]!;
                } else {
                    break;
                }
            } while (en.MoveNext());

            var methodResolver = new MethodResolver(targetObj, MethodBindingAttr);
            if (optionOrAction == "--help" || optionOrAction == "-h")
            {
                HandleInstanceHelp(cliOptions, methodResolver);
                return null;
            }
            if (optionOrAction == "--version" || optionOrAction == "-v") {
                Console.WriteLine(Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3));
                return null;
            }

            var cliAction = methodResolver.GetMethod(optionOrAction);
            var actionParams = new CliActionParametersStore(cliAction, MethodBindingAttr);
            var optionsAndActionParams = new CliOptionsAndActionParametersStore(cliOptions, actionParams, targetObj);
            if (TryReadOptionsAndActionParams(en, optionsAndActionParams))
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
