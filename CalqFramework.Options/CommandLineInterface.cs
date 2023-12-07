using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static CalqFramework.Options.OptionsReaderBase;

namespace CalqFramework.Options {
    public class CommandLineInterface {

    static object GetDefaultValue(ParameterInfo parameter) => parameter.HasDefaultValue ? parameter.DefaultValue : "N/A";

    public static object? Execute(object instance, string[] args) {
            return Execute(instance, args, new CliSerializerOptions() { SkipUnknown = true }); // FIXME - set SkipUnknown inside Execute method - maybe add public CliSerializerOptions(CliSerializerOptions options)
        }

        public static object? Execute(object instance, string[] args, CliSerializerOptions options) {

            object[] ReadOptions(ParameterInfo[] parameters, string[] args, ICollection<string> whiteList, int startIndex) {
                var assignedParams = new HashSet<ParameterInfo>();

                var paramValues = new object?[parameters.Length];
                var reader = new ToMethodOptionsReader(parameters);
                var i = 0;
                foreach (var (option, value, optionAttr) in reader.Read(args, startIndex)) {
                    whiteList = whiteList.Select(x => ToMethodOptionsReader.GetOptionName(parameters, x)).ToHashSet(); // assure that Reader also return unresolved option names

                    void Assign() {
                        if (optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                            if (i >= parameters.Length) {
                                throw new Exception("passed too many args");
                            }
                            paramValues[i] = ValueParser.ParseValue(option, parameters[i].ParameterType, parameters[i].Name);
                            assignedParams.Add(parameters[i]);
                            ++i;
                        } else {
                            var (parameter, index) = GetParameterIndexPair(parameters, option);
                            var valueObj = ValueParser.ParseValue(value, parameter.ParameterType, option);
                            paramValues[index] = valueObj;
                            assignedParams.Add(parameter);
                        }
                    }

                    if ((option == "help" || option == "h") && !optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                        foreach (var parameter in parameters) {
                            Console.WriteLine($"--{parameter.Name}, -{parameter.Name![0]} - Type: {parameter.ParameterType}, Required: {!parameter.IsOptional}, Default: {GetDefaultValue(parameter)}");
                        }
                        return Array.Empty<object>();
                    }

                    if (optionAttr.HasFlag(OptionFlags.ValueUnassigned)) {
                        if (whiteList.Contains(option) && !optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                            throw new Exception($"unknown option {option}");
                        }
                        Assign();
                    } else {
                        if (!whiteList.Contains(option)) {
                            throw new Exception($"internal error - instance option and method option are the same {option}"); // TODO do not assume this error message - just throw its not whitelisted
                        }
                        Assign();
                    }
                }

                var assignedParamNames = assignedParams.Select(x => x.Name).ToHashSet();
                for (var j = 0; j < parameters.Length; ++j) {
                    var param = parameters[j];
                    if (!assignedParamNames.Contains(param.Name)) {
                        if (!param.IsOptional) {
                            throw new Exception($"unassigned option {param.Name}");
                        }
                        paramValues[j] = param.DefaultValue!;
                    }
                }

                return paramValues;
            }

            (ParameterInfo parameter, int index) GetParameterIndexPair(ParameterInfo[] parameters, string option) {
                for (var i = 0; i < parameters.Length; ++i) {
                    var param = parameters[i];
                    if (param.Name == option) {
                        return (param, i);
                    }
                }
                throw new Exception($"option doesn't exist: {option}");
            }

            var dataMemberAccessor = DataMemberAccess.CliDataMemberAccessorFactory.Instance.CreateDataMemberAccessor(options);

            var obj = instance;
            var i = 0;
            try {
                for (; i < args.Length; ++i) {
                    var type = dataMemberAccessor.GetDataMemberType(obj.GetType(), args[i]);
                    if (type.IsPrimitive || type == typeof(string)) { // TODO if parseable
                        throw new Exception($"{args[i]} is not a core command");
                    }
                    obj = dataMemberAccessor.GetDataMemberValue(instance, args[i]); // TODO TryGetDataMemberValue
                }
            } catch (MissingMemberException) {
                if (args[i] == "--help" || args[i] == "-h") { // TODO create HelpData Help() and void PrintHelp(HelpData)
                    var membersByKeys = dataMemberAccessor.GetDataMembersByKeys(obj.GetType());
                    var keysByMembers = membersByKeys.GroupBy(x => x.Value, x => x.Key).ToDictionary(x => x.Key, x => x.OrderByDescending(e => e.Length).ToList());
                    var globalOptions = keysByMembers.Where(x => { var type = dataMemberAccessor.GetDataMemberType(obj.GetType(), x.Value[0]); return type.IsPrimitive || type == typeof(string); }); // TODO if parseable
                    var coreCommands = keysByMembers.Where(x => { var type = dataMemberAccessor.GetDataMemberType(obj.GetType(), x.Value[0]); return !type.IsPrimitive && type != typeof(string); }); // TODO if parseable
                    var actionCommands = obj.GetType().GetMethods(options.BindingAttr);
                    // TODO move to separate method
                    // TODO get sumnmaries from documentation
                    Console.WriteLine("CORE COMMANDS:");
                    foreach (var command in coreCommands) {
                        Console.WriteLine($"Name: {command.Value}");
                    }
                    Console.WriteLine();
                    Console.WriteLine("ACTION COMMANDS:");
                    foreach (var methodInfo in actionCommands) {
                        Console.WriteLine($"{methodInfo.Name}({string.Join(", ", methodInfo.GetParameters().Select(x => $"{x.ParameterType.Name} {x.Name}{(x.HasDefaultValue ? $" = {x.DefaultValue}" : "")}"))})"); // TODO method/param accessor to separate this logic
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

                var skippedOptions = OptionsDeserializer.Deserialize(instance, options, args, i);

                var parameters = method.GetParameters();
                var parameterValues = ReadOptions(parameters, args, skippedOptions, i);

                return method.Invoke(obj, parameterValues);
            }

            throw new Exception($"no command specified");
        }
    }
}
