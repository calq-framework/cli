using CalqFramework.Cli.DataAccess;
using CalqFramework.Cli.DataAccess.DataMemberAccess;
using System;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.Serialization.Parsing;
using static CalqFramework.Cli.Serialization.Parsing.OptionsReaderBase;
using CalqFramework.Cli.Serialization;
using System.Collections.Generic;
using CalqFramework.Serialization.DataAccess.DataMemberAccess;

namespace CalqFramework.Cli {
    public class CommandLineInterface
    {
        private static object GetDefaultValue(ParameterInfo parameter) => parameter.HasDefaultValue ? parameter.DefaultValue! : "N/A";

        // TODO separate data and printing
        private static void HandleMethodHelp(DataMemberAndMethodParamAccessor accessor)
        {
            foreach (var parameter in accessor.Parameters)
            {
                Console.WriteLine($"--{parameter.Name}, -{parameter.Name![0]} - Type: {parameter.ParameterType}, Required: {!parameter.IsOptional}, Default: {GetDefaultValue(parameter)}");
            }
        }

        // TODO separate data and printing
        private static void HandleInstanceHelp(DataMemberAndMethodAccessor accessor)
        {
            var membersByKeys = accessor.GetDataMembersByKeys();
            var keysByMembers = membersByKeys.GroupBy(x => x.Value, x => x.Key).ToDictionary(x => x.Key, x => x.OrderByDescending(e => e.Length).ToList());
            var globalOptions = keysByMembers.Where(x =>
            {
                var type = accessor.GetType(x.Value[0]);
                return type.IsPrimitive || type == typeof(string); // TODO create IsParseable()
            });
            var coreCommands = keysByMembers.Where(x =>
            {
                var type = accessor.GetType(x.Value[0]);
                return !type.IsPrimitive && type != typeof(string); // TODO create IsParseable()
            });

            Console.WriteLine("CORE COMMANDS:");
            foreach (var command in coreCommands)
            {
                Console.WriteLine($"{command.Value[0]}");
            }

            Console.WriteLine();
            Console.WriteLine("ACTION COMMANDS:");
            foreach (var methodInfo in accessor.Methods)
            {
                Console.WriteLine($"{methodInfo.Name}({string.Join(", ", methodInfo.GetParameters().Select(x => $"{x.ParameterType.Name} {x.Name}{(x.HasDefaultValue ? $" = {x.DefaultValue}" : "")}"))})");
            }

            Console.WriteLine();
            Console.WriteLine("GLOBAL OPTIONS:");
            foreach (var option in globalOptions)
            {
                var type = accessor.GetType(option.Value[0]);
                var defaultValue = accessor.GetValue(option.Value[0]);
                Console.WriteLine($"--{string.Join(", -", option.Value)} [{type.Name}: {defaultValue}]");
            }
        }

        private static bool TryReadOptions(IEnumerator<string> args, DataMemberAndMethodParamAccessor accessor, CliDeserializerOptions options)
        {
            var optionsReader = new OptionsReader(args, accessor);

            var parameterIndex = 0;
            void SetPositionalParameter(string option)
            {
                var parameterName = accessor.GetKey(parameterIndex);
                var parameterType = accessor.GetType(parameterIndex);
                accessor.SetValue(parameterIndex, ValueParser.ParseValue(option, parameterType, parameterName));
                ++parameterIndex;
            }

            try
            {
                foreach (var (option, value, optionAttr) in optionsReader.Read())
                {
                    if ((option == "help" || option == "h") && !optionAttr.HasFlag(OptionFlags.NotAnOption))
                    {
                        HandleMethodHelp(accessor);
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
                        var type = accessor.GetType(option);
                        var valueObj = ValueParser.ParseValue(value, type, option);
                        accessor.SetOrAddValue(option, valueObj);
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

            var nestedObj = obj;
            var dataMemberAccessorFactory = new AliasableDataMemberAccessorFactory(options.DataMemberAccessorOptions);
            IDataMemberAccessor dataMemberAccessor;

            // explore object tree until optionOrAction definitely cannot be an action (object not found by name)
            do {
                optionOrAction = en.Current;
                dataMemberAccessor = dataMemberAccessorFactory.CreateDataMemberAccessor(nestedObj);
                if (dataMemberAccessor.HasKey(optionOrAction)) {
                    var type = dataMemberAccessor.GetType(optionOrAction);
                    if (type.IsPrimitive || type == typeof(string)) {
                        break;
                    }
                    nestedObj = dataMemberAccessor.GetValue(optionOrAction)!;
                } else {
                    break;
                }
            } while (en.MoveNext());

            var dataMemberAndMethodAccessor = new DataMemberAndMethodAccessor(dataMemberAccessor, options.MethodBindingAttr);
            if (optionOrAction == "--help" || optionOrAction == "-h")
            {
                HandleInstanceHelp(dataMemberAndMethodAccessor);
                return null;
            }

            var method = dataMemberAndMethodAccessor.GetMethod(optionOrAction);
            var methodParamsAccessor = new MethodParamAccessor(method);
            var accessor = new DataMemberAndMethodParamAccessor(dataMemberAccessor, methodParamsAccessor);
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
