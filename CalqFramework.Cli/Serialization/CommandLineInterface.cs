using CalqFramework.Cli.DataAccess;
using CalqFramework.Cli.DataAccess.DataMemberAccess;
using System;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.Serialization.Parsing;
using static CalqFramework.Cli.Serialization.Parsing.OptionsReaderBase;

namespace CalqFramework.Cli.Serialization
{
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
        private static void HandleInstanceHelp(DataMemberAndMethodAccessor objectAccessor)
        {
            var membersByKeys = objectAccessor.GetDataMembersByKeys();
            var keysByMembers = membersByKeys.GroupBy(x => x.Value, x => x.Key).ToDictionary(x => x.Key, x => x.OrderByDescending(e => e.Length).ToList());
            var globalOptions = keysByMembers.Where(x =>
            {
                var type = objectAccessor.GetType(x.Value[0]);
                return type.IsPrimitive || type == typeof(string); // TODO create IsParseable()
            });
            var coreCommands = keysByMembers.Where(x =>
            {
                var type = objectAccessor.GetType(x.Value[0]);
                return !type.IsPrimitive && type != typeof(string); // TODO create IsParseable()
            });

            Console.WriteLine("CORE COMMANDS:");
            foreach (var command in coreCommands)
            {
                Console.WriteLine($"Name: {command.Value}");
            }

            Console.WriteLine();
            Console.WriteLine("ACTION COMMANDS:");
            foreach (var methodInfo in objectAccessor.Methods)
            {
                Console.WriteLine($"{methodInfo.Name}({string.Join(", ", methodInfo.GetParameters().Select(x => $"{x.ParameterType.Name} {x.Name}{(x.HasDefaultValue ? $" = {x.DefaultValue}" : "")}"))})");
            }

            Console.WriteLine();
            Console.WriteLine("GLOBAL OPTIONS:");
            foreach (var option in globalOptions)
            {
                var type = objectAccessor.GetType(option.Value[0]);
                var defaultValue = objectAccessor.GetValue(option.Value[0]);
                Console.WriteLine($"--{string.Join(", =", option.Value)} - Type: {type}, Default: {defaultValue}");
            }
        }

        private static bool TryReadOptions(string[] args, int index, DataMemberAndMethodParamAccessor accessor)
        {
            var optionsReader = new OptionsReader(args, accessor) { StartIndex = index };
            var parameterIndex = 0;

            try {
                foreach (var (option, value, optionAttr) in optionsReader.Read()) {
                    if ((option == "help" || option == "h") && !optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                        HandleMethodHelp(accessor);
                        return false;
                    }

                    if (optionAttr.HasFlag(OptionFlags.ValueUnassigned) && !optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                        throw new CliException($"unknown option {option}");
                    }

                    if (optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                        var parameterName = accessor.GetKey(parameterIndex);
                        var parameterType = accessor.GetType(parameterIndex);
                        accessor.SetValue(parameterIndex, ValueParser.ParseValue(option, parameterType, parameterName));
                        ++parameterIndex;
                    } else {
                        var type = accessor.GetType(option);
                        var valueObj = ValueParser.ParseValue(value, type, option);
                        accessor.SetOrAddValue(option, valueObj);
                    }
                }
            } catch (Exception ex) { // TODO rename the exception in CalqFramework.Serialization
                if (ex.Message == "collision") {
                    throw new CliException(ex.Message, ex);
                }
                throw;
            }

            return true;
        }

        public static object? Execute(object targetObj, string[] args)
        {
            return Execute(targetObj, args, new CliDeserializerOptions());
        }

        public static object? Execute(object targetObj, string[] args, CliDeserializerOptions options)
        {
            var dataMemberAccessorFactory = new DataMemberAccessorFactory(options.DataMemberAccessorOptions);

            var obj = targetObj;
            var index = 0;

            var dataMemberAccessor = dataMemberAccessorFactory.CreateDataMemberAccessor(obj);
            var optionOrAction = args[index];

            // explore object tree until optionOrAction definitely cannot be an action (object not found by name)
            for (; index < args.Length; )
            {
                if (dataMemberAccessor.HasKey(optionOrAction))
                {
                    var type = dataMemberAccessor.GetType(optionOrAction);
                    if (type.IsPrimitive || type == typeof(string))
                    {
                        throw new CliException($"{optionOrAction} is not a core command");
                    }
                    obj = dataMemberAccessor.GetValue(optionOrAction)!;
                }
                else
                {
                    break;
                }
                dataMemberAccessor = dataMemberAccessorFactory.CreateDataMemberAccessor(obj);
                optionOrAction = args[++index];
            }

            var objectAccessor = new DataMemberAndMethodAccessor(dataMemberAccessor, options.MethodBindingAttr);
            if (optionOrAction == "--help" || optionOrAction == "-h")
            {
                HandleInstanceHelp(objectAccessor);
                return null;
            }

            var methodParamsAccessor = new MethodParamAccessor(obj, optionOrAction, options.MethodBindingAttr);
            var accessor = new DataMemberAndMethodParamAccessor(dataMemberAccessor, methodParamsAccessor);
            if (TryReadOptions(args, ++index, accessor))
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
