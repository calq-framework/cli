using CalqFramework.Cli.Serialization.Parsing;
using System;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess {
    internal class CliMethodParamAccessor : MethodParamAccessorBase {
        public CliMethodParamAccessor(MethodInfo method, BindingFlags bindingAttr) : base(method, bindingAttr) {
        }

        public override string ToString() {
            var result = "";

            result += "[POSITIONAL PARAMETERS]\n";
            foreach (var parameter in Parameters) {
                result += $"{DataMediatorToString(parameter)} # {ToStringHelper.GetTypeName(parameter.ParameterType)} {ToStringHelper.GetDefaultValue(parameter)}\n";
            }

            // TODO DRY with HandleInstanceHelp
            var members = DataMediators.ToList();
            var coreCommandOptions = members.Where(x => {
                return ValueParser.IsParseable(GetDataType(x));
            });

            result += "\n";
            result += "[OPTIONS]\n";
            foreach (var option in coreCommandOptions) {
                var type = GetDataType(option);
                var defaultValue = this[option];
                result += $"{DataMediatorToString(option)} # {ToStringHelper.GetTypeName(type)} ({defaultValue})\n";
            }

            return result;
        }
    }
}