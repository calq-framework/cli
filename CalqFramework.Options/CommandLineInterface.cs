using CalqFramework.Options.DataMemberAccess;
using CalqFramework.Serialization.DataMemberAccess;
using System;
using System.Linq;
using System.Reflection;
using static CalqFramework.Options.OptionsReaderBase;

namespace CalqFramework.Options {
    public class CommandLineInterface {

        private static object GetDefaultValue(ParameterInfo parameter) => parameter.HasDefaultValue ? parameter.DefaultValue : "N/A";

        public static object? Execute(object targetObj, string[] args) {
            return Execute(targetObj, args, new CliSerializerOptions());
        }

        // TODO use accessors (remove Parameters)
        // TODO separate data and printing
        private static void HandleMethodHelp(MethodParamsAccessor methodParamsAccessor) {
            foreach (var parameter in methodParamsAccessor.Parameters) {
                Console.WriteLine($"--{parameter.Name}, -{parameter.Name![0]} - Type: {parameter.ParameterType}, Required: {!parameter.IsOptional}, Default: {GetDefaultValue(parameter)}");
            }
            throw new InvokedHelpException();
        }

        // TODO use accessors only (remove obj.GetType().GetMethods)
        // TODO separate data and printing
        private static void HandleInstanceHelp(IDataMemberAccessor dataMemberAccessor, object obj, CliSerializerOptions options) {
            var membersByKeys = dataMemberAccessor.GetDataMembersByKeys(obj.GetType());
            var keysByMembers = membersByKeys.GroupBy(x => x.Value, x => x.Key).ToDictionary(x => x.Key, x => x.OrderByDescending(e => e.Length).ToList());
            var globalOptions = keysByMembers.Where(x => {
                var type = dataMemberAccessor.GetDataMemberType(obj.GetType(), x.Value[0]);
                return type.IsPrimitive || type == typeof(string); // TODO create IsParseable()
            });
            var coreCommands = keysByMembers.Where(x => {
                var type = dataMemberAccessor.GetDataMemberType(obj.GetType(), x.Value[0]);
                return !type.IsPrimitive && type != typeof(string); // TODO create IsParseable()
            });
            var actionCommands = obj.GetType().GetMethods(options.BindingAttr);

            Console.WriteLine("CORE COMMANDS:");
            foreach (var command in coreCommands) {
                Console.WriteLine($"Name: {command.Value}");
            }

            Console.WriteLine();
            Console.WriteLine("ACTION COMMANDS:");
            foreach (var methodInfo in actionCommands) {
                Console.WriteLine($"{methodInfo.Name}({string.Join(", ", methodInfo.GetParameters().Select(x => $"{x.ParameterType.Name} {x.Name}{(x.HasDefaultValue ? $" = {x.DefaultValue}" : "")}"))})");
            }

            Console.WriteLine();
            Console.WriteLine("GLOBAL OPTIONS:");
            foreach (var option in globalOptions) {
                var type = dataMemberAccessor.GetDataMemberType(obj.GetType(), option.Value[0]);
                var defaultValue = dataMemberAccessor.GetDataMemberValue(obj, option.Value[0]);
                Console.WriteLine($"--{string.Join(", =", option.Value)} - Type: {type}, Default: {defaultValue}");
            }
        }

        private static void ReadOptions(ToMethodOptionsReader optionsReader) {
            var currentIndex = 0;
            var accessor = optionsReader.DataMemberAndMethodParamsAccessor;
            var parameters = accessor.MethodParamsAccessor.Parameters;

            foreach (var (option, value, optionAttr) in optionsReader.Read()) {
                if ((option == "help" || option == "h") && !optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                    HandleMethodHelp(accessor.MethodParamsAccessor);
                }

                if (optionAttr.HasFlag(OptionFlags.ValueUnassigned) && !optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                    throw new Exception($"unknown option {option}");
                }

                if (optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                    var parameterName = accessor.MethodParamsAccessor.ResolveDataMemberKey(currentIndex);
                    var parameterType = accessor.MethodParamsAccessor.GetParameterType(currentIndex);
                    accessor.MethodParamsAccessor.SetDataValue(currentIndex, ValueParser.ParseValue(option, parameterType, parameterName));
                    ++currentIndex;
                } else {
                    accessor.TryGetDataType(option, out var type);
                    var valueObj = ValueParser.ParseValue(value, type, option);
                    accessor.SetDataValue(option, valueObj);
                }
            }

            // TODO move to MethodParamsAccessor.Invoke()
            var assignedParamNames = accessor.MethodParamsAccessor.AssignedParameters.Select(x => x.Name).ToHashSet();
            for (var j = 0; j < parameters.Length; ++j) {
                var param = parameters[j];
                if (!assignedParamNames.Contains(param.Name)) {
                    if (!param.IsOptional) {
                        throw new Exception($"unassigned option {param.Name}");
                    }
                    accessor.MethodParamsAccessor.SetDataValue(j, param.DefaultValue!);
                }
            }
        }

        public static object? Execute(object targetObj, string[] args, CliSerializerOptions options) {
            var dataMemberAccessor = CliDataMemberAccessorFactory.targetObj.CreateDataMemberAccessor(options);

            var currentObj = targetObj;
            var currentIndex = 0;

            try {
                for (; currentIndex < args.Length; ++currentIndex) {
                    var type = dataMemberAccessor.GetDataMemberType(currentObj.GetType(), args[currentIndex]);
                    if (type.IsPrimitive || type == typeof(string)) {
                        throw new Exception($"{args[currentIndex]} is not a core command");
                    }
                    currentObj = dataMemberAccessor.GetDataMemberValue(targetObj, args[currentIndex])!;
                }
            } catch (MissingMemberException) {
                if (args[currentIndex] == "--help" || args[currentIndex] == "-h") {
                    HandleInstanceHelp(dataMemberAccessor, currentObj, options);
                    return null;
                }

                var methodParamsAccessor = new MethodParamsAccessor(currentObj, args[currentIndex], options.BindingAttr);
                ++currentIndex;

                var accessor = new DataMemberAndMethodParamsAccessor(
                        currentObj,
                        dataMemberAccessor,
                        methodParamsAccessor
                    );
                var optionsReader = new ToMethodOptionsReader(args, accessor) { StartIndex = currentIndex };
                try {
                    ReadOptions(optionsReader);
                    return methodParamsAccessor.Invoke();
                } catch (InvokedHelpException) {
                    return null;
                }
            }

            throw new Exception($"no command specified");
        }
    }
}
