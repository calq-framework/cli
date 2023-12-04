using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static CalqFramework.Options.OptionsReaderBase;

namespace CalqFramework.Options {
    public class CommandLineInterface {

        public static object? Execute(object instance, string[] args) {
            return Execute(instance, args, new CliSerializerOptions() { SkipUnknown = true });
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

                    if (!optionAttr.HasFlag(OptionFlags.ValueUnassigned)) {
                        if (whiteList.Contains(option)) {
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
                    obj = dataMemberAccessor.GetDataMemberValue(instance, args[i]); // TODO TryGetDataMemberValue
                }
            } catch (MissingMemberException) {
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
