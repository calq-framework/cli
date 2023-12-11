using CalqFramework.Options.DataMemberAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static CalqFramework.Options.OptionsReaderBase;

namespace CalqFramework.Options {
    public class CommandLineInterface {

        static object GetDefaultValue(ParameterInfo parameter) => parameter.HasDefaultValue ? parameter.DefaultValue : "N/A";

        public static object? Execute(object instance, string[] args) {
            return Execute(instance, args, new CliSerializerOptions() { SkipUnknown = true });
        }

        public static object? Execute(object instance, string[] args, CliSerializerOptions options) {

            object[] ReadOptions(ParameterInfo[] parameters, string[] args, int startIndex) {
                var methodAccessor = new MethodParamsAccessor(instance, parameters);
                var accessor = new DataMemberAndMethodParamsAccessor(
                    instance,
                    DataMemberAccess.CliDataMemberAccessorFactory.Instance.CreateDataMemberAccessor(options),
                    methodAccessor
                );
                var reader = new ToMethodOptionsReader(accessor);
                var i = 0;

                foreach (var (option, value, optionAttr) in reader.Read(args, startIndex)) {
                    if ((option == "help" || option == "h") && !optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                        foreach (var parameter in parameters) {
                            Console.WriteLine($"--{parameter.Name}, -{parameter.Name![0]} - Type: {parameter.ParameterType}, Required: {!parameter.IsOptional}, Default: {GetDefaultValue(parameter)}");
                        }
                        return Array.Empty<object>();
                    }

                    if (optionAttr.HasFlag(OptionFlags.ValueUnassigned) && !optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                        throw new Exception($"unknown option {option}");
                    }

                    if (optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                        if (i >= parameters.Length) {
                            throw new Exception("passed too many args");
                        }
                        accessor.SetDataValue(i, ValueParser.ParseValue(option, parameters[i].ParameterType, parameters[i].Name));
                        ++i;
                    } else {
                        accessor.TryGetDataType(option, out var type);
                        var valueObj = ValueParser.ParseValue(value, type, option);
                        accessor.SetDataValue(option, valueObj);
                    }
                }

                var assignedParamNames = methodAccessor.AssignedParameters.Select(x => x.Name).ToHashSet();

                for (var j = 0; j < parameters.Length; ++j) {
                    var param = parameters[j];
                    if (!assignedParamNames.Contains(param.Name)) {
                        if (!param.IsOptional) {
                            throw new Exception($"unassigned option {param.Name}");
                        }
                        accessor.SetDataValue(j, param.DefaultValue!);
                    }
                }

                return methodAccessor.ParamValues;
            }

            var dataMemberAccessor = DataMemberAccess.CliDataMemberAccessorFactory.Instance.CreateDataMemberAccessor(options);

            var obj = instance;
            var i = 0;

            try {
                for (; i < args.Length; ++i) {
                    var type = dataMemberAccessor.GetDataMemberType(obj.GetType(), args[i]);
                    if (type.IsPrimitive || type == typeof(string)) {
                        throw new Exception($"{args[i]} is not a core command");
                    }
                    obj = dataMemberAccessor.GetDataMemberValue(instance, args[i]);
                }
            } catch (MissingMemberException) {
                if (args[i] == "--help" || args[i] == "-h") {
                    var membersByKeys = dataMemberAccessor.GetDataMembersByKeys(obj.GetType());
                    var keysByMembers = membersByKeys.GroupBy(x => x.Value, x => x.Key).ToDictionary(x => x.Key, x => x.OrderByDescending(e => e.Length).ToList());
                    var globalOptions = keysByMembers.Where(x => {
                        var type = dataMemberAccessor.GetDataMemberType(obj.GetType(), x.Value[0]);
                        return type.IsPrimitive || type == typeof(string);
                    });
                    var coreCommands = keysByMembers.Where(x => {
                        var type = dataMemberAccessor.GetDataMemberType(obj.GetType(), x.Value[0]);
                        return !type.IsPrimitive && type != typeof(string);
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

                    return null;
                }

                var method = obj.GetType().GetMethod(args[i], options.BindingAttr);
                if (method == null) {
                    throw new Exception($"invalid command");
                }
                ++i;

                var parameters = method.GetParameters();
                var parameterValues = ReadOptions(parameters, args, i);

                return method.Invoke(obj, parameterValues);
            }

            throw new Exception($"no command specified");
        }
    }
}
