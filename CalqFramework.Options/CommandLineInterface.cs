using System;
using System.Reflection;
using static CalqFramework.Options.OptionsReaderBase;

namespace CalqFramework.Options {
    public class CommandLineInterface {

        public static object? Execute(object instance, string[] args) {
            return Execute(instance, args, new CliSerializerOptions());
        }
        
        public static object? Execute(object instance, string[] args, CliSerializerOptions options) {

            int ReadOptions(ParameterInfo[] parameters, string[] args, int startIndex, ref object[] paramValues) {
                var reader = new ToMethodOptionsReader(parameters);
                var nolongerPositional = false;
                var i = 0;
                foreach (var (option, value, optionAttr) in reader.Read(args, startIndex)) {
                    if (optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                        if (nolongerPositional) {
                            throw new Exception("unexpected value");
                        } else {
                            if (i >= parameters.Length) {
                                throw new Exception("passed too many args");
                            }
                            paramValues[i] = ValueParser.ParseValue(option, parameters[i].ParameterType, parameters[i].Name);
                            ++i;
                            continue;
                        }
                    } else {
                        nolongerPositional = true;
                    }

                    var (parameter, index) = GetParameterIndexPair(parameters, option);
                    var valueObj = ValueParser.ParseValue(value, parameter.ParameterType, option);
                    paramValues[index] = valueObj;
                }

                return reader.LastIndex;
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
                    obj = dataMemberAccessor.GetDataMemberValue(instance, args[i]);
                }
            } catch (MissingMemberException) {
                //var methodBindingFlags = BindingFlags.Public | BindingFlags.Instance;
                //methodBindingFlags = deserializerOptions.HasFlag(CommandLineInterfaceOptions.IgnoreCase) ? methodBindingFlags | BindingFlags.IgnoreCase : methodBindingFlags;
                var method = obj.GetType().GetMethod(args[i], options.BindingAttr);
                if (method == null) {
                    throw new Exception($"invalid command");
                }

                ++i;
                var parameters = method.GetParameters();
                var parameterValues = new object[method.GetParameters().Length];
                ReadOptions(parameters, args, i, ref parameterValues);

                return method.Invoke(obj, parameterValues);
            }

            throw new Exception($"no command specified");
        }
    }
}
