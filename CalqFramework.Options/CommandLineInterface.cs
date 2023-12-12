using CalqFramework.Options.DataMemberAccess;
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

        private static object[] ReadOptions(ToMethodOptionsReader optionsReader) { //object targetObj, ParameterInfo[] parameters, string[] args, int startIndex) {
            var currentIndex = 0;
            var accessor = optionsReader.DataMemberAndMethodParamsAccessor;
            var parameters = accessor.MethodParamsAccessor.Parameters;

            foreach (var (option, value, optionAttr) in optionsReader.Read()) {
                if ((option == "help" || option == "h") && !optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                    foreach (var parameter in parameters) {
                        Console.WriteLine($"--{parameter.Name}, -{parameter.Name![0]} - Type: {parameter.ParameterType}, Required: {!parameter.IsOptional}, Default: {GetDefaultValue(parameter)}");
                    }
                    throw new InvokedHelpException();
                }

                if (optionAttr.HasFlag(OptionFlags.ValueUnassigned) && !optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                    throw new Exception($"unknown option {option}");
                }

                if (optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                    if (currentIndex >= parameters.Length) {
                        throw new Exception("passed too many args");
                    }
                    accessor.MethodParamsAccessor.SetDataValue(currentIndex, ValueParser.ParseValue(option, parameters[currentIndex].ParameterType, parameters[currentIndex].Name));
                    ++currentIndex;
                } else {
                    accessor.TryGetDataType(option, out var type);
                    var valueObj = ValueParser.ParseValue(value, type, option);
                    accessor.SetDataValue(option, valueObj);
                }
            }

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

            return accessor.MethodParamsAccessor.ParamValues;
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
                    currentObj = dataMemberAccessor.GetDataMemberValue(targetObj, args[currentIndex]);
                }
            } catch (MissingMemberException) {
                if (args[currentIndex] == "--help" || args[currentIndex] == "-h") {
                    var membersByKeys = dataMemberAccessor.GetDataMembersByKeys(currentObj.GetType());
                    var keysByMembers = membersByKeys.GroupBy(x => x.Value, x => x.Key).ToDictionary(x => x.Key, x => x.OrderByDescending(e => e.Length).ToList());
                    var globalOptions = keysByMembers.Where(x => {
                        var type = dataMemberAccessor.GetDataMemberType(currentObj.GetType(), x.Value[0]);
                        return type.IsPrimitive || type == typeof(string);
                    });
                    var coreCommands = keysByMembers.Where(x => {
                        var type = dataMemberAccessor.GetDataMemberType(currentObj.GetType(), x.Value[0]);
                        return !type.IsPrimitive && type != typeof(string);
                    });
                    var actionCommands = currentObj.GetType().GetMethods(options.BindingAttr);

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
                        var type = dataMemberAccessor.GetDataMemberType(currentObj.GetType(), option.Value[0]);
                        var defaultValue = dataMemberAccessor.GetDataMemberValue(currentObj, option.Value[0]);
                        Console.WriteLine($"--{string.Join(", =", option.Value)} - Type: {type}, Default: {defaultValue}");
                    }

                    return null;
                }

                var method = currentObj.GetType().GetMethod(args[currentIndex], options.BindingAttr);
                if (method == null) {
                    throw new Exception($"invalid command");
                }
                ++currentIndex;

                try {
                    var parameters = method.GetParameters();
                    var methodParamsAccessor = new MethodParamsAccessor(targetObj, parameters);
                    var accessor = new DataMemberAndMethodParamsAccessor(
                        targetObj,
                        dataMemberAccessor,
                        methodParamsAccessor
                    );
                    var optionsReader = new ToMethodOptionsReader(args, accessor) { StartIndex = currentIndex };
                    var parameterValues = ReadOptions(optionsReader); //(parameters, args, currentIndex);

                    return method.Invoke(currentObj, parameterValues);
                } catch (InvokedHelpException) {
                    return null;
                }
            }

            throw new Exception($"no command specified");
        }
    }
}
