using CalqFramework.Cli.DataAccess;
using CalqFramework.Cli.DataAccess.DataMemberAccess;
using System;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.Serialization.Parsing;
using static CalqFramework.Cli.Serialization.Parsing.OptionsReaderBase;
using CalqFramework.Cli.Serialization;
using System.Collections.Generic;
using CalqFramework.Serialization.DataAccess;

namespace CalqFramework.Cli {
    public class CommandLineInterface
    {
        // TODO separate data and printing
        private static void HandleMethodHelp(DataMemberAndMethodParamAccessor accessor, CliDeserializerOptions options)
        {
            
        }

        // TODO separate data and printing
        private static void HandleInstanceHelp(ICliDataMemberAccessor accessor, MethodResolver methodResolver, CliDeserializerOptions options)
        {
            Console.WriteLine(accessor.CliSerializer.GetHelpString());

            Console.WriteLine("[ACTION COMMANDS]");
            foreach (var methodInfo in methodResolver.Methods) {
                Console.WriteLine(methodResolver.MethodToString(methodInfo));
            }
        }

        private static bool TryReadOptions(IEnumerator<string> args, DataMemberAndMethodParamAccessor accessor, CliDeserializerOptions options)
        {
            var optionsReader = new OptionsReader(args, accessor);

            var parameterIndex = 0;
            void SetPositionalParameter(string option)
            {
                var parameterName = accessor.MethodParamsAccessor.GetKey(parameterIndex);
                var parameterType = accessor.MethodParamsAccessor.GetType(parameterIndex);
                accessor.MethodParamsAccessor.SetValue(parameterIndex, ValueParser.ParseValue(option, parameterType, parameterName));
                ++parameterIndex;
            }

            try
            {
                foreach (var (option, value, optionAttr) in optionsReader.Read())
                {
                    if ((option == "help" || option == "h") && !optionAttr.HasFlag(OptionFlags.NotAnOption))
                    {
                        HandleMethodHelp(accessor, options);
                        return false;
                    }

                    if (optionAttr.HasFlag(OptionFlags.ValueUnassigned) && !optionAttr.HasFlag(OptionFlags.NotAnOption))
                    {
                        if (options.SkipUnknown) {
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
                        var type = accessor.GetDataType(option);
                        var valueObj = ValueParser.ParseValue(value, type, option);
                        accessor[option] = valueObj;
                    }
                }
                while (args.MoveNext()) {
                    SetPositionalParameter(args.Current);
                }
            }
            catch (Exception ex)
            { // TODO rename the exception in CalqFramework.Serialization
                if (ex.Message == "collision")
                {
                    throw new CliException(ex.Message, ex);
                }
                throw;
            }

            return true;
        }

        public static object? Execute(object obj) {
            return Execute(obj, new CliDeserializerOptions());
        }

        public static object? Execute(object obj, CliDeserializerOptions options) {
            return Execute(obj, options, Environment.GetCommandLineArgs().Skip(1));
        }

        public static object? Execute(object obj, IEnumerable<string> args)
        {
            return Execute(obj, new CliDeserializerOptions(), args);
        }

        public static object? Execute(object obj, CliDeserializerOptions options, IEnumerable<string> args)
        {
            using var en = args.GetEnumerator();
            if (!en.MoveNext()) {
                return null;
            }
            string optionOrAction;

            var targetObj = obj;
            var dataMemberAccessorFactory = new CliDataMemberAccessorFactory(options.DataMemberAccessorOptions);
            ICliDataMemberAccessor dataMemberAccessor;

            // explore object tree until optionOrAction definitely cannot be an action (object not found by name)
            do {
                optionOrAction = en.Current;
                dataMemberAccessor = dataMemberAccessorFactory.CreateCliAccessor(targetObj);
                if (dataMemberAccessor.ContainsKey(optionOrAction)) {
                    var type = dataMemberAccessor.GetDataType(optionOrAction);
                    if (ValueParser.IsParseable(type)) {
                        break;
                    }
                    targetObj = dataMemberAccessor[optionOrAction]!;
                } else {
                    break;
                }
            } while (en.MoveNext());

            var methodResolver = new MethodResolver(targetObj, options.MethodBindingAttr);
            if (optionOrAction == "--help" || optionOrAction == "-h")
            {
                HandleInstanceHelp(dataMemberAccessor, methodResolver, options);
                return null;
            }
            if (optionOrAction == "--version" || optionOrAction == "-v") {
                Console.WriteLine(Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3));
                return null;
            }

            var method = methodResolver.GetMethod(optionOrAction);
            var methodParamsAccessor = new CliMethodParamAccessor(method, options.MethodBindingAttr);
            var accessor = new DataMemberAndMethodParamAccessor(dataMemberAccessor, methodParamsAccessor, targetObj);
            if (TryReadOptions(en, accessor, options))
            {
                return accessor.Invoke();
            }
            else
            {
                return null;
            }

            throw new CliException($"no command specified");
        }
    }
}
