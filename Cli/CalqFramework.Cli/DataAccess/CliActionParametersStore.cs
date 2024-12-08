using CalqFramework.Cli.Serialization.Parsing;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess {
    // TODO move this logic into serializer
    internal class CliActionParametersStore : MethodParamStoreBase {
        public CliActionParametersStore(MethodInfo method, BindingFlags bindingAttr) : base(method, bindingAttr) {
        }

        public static string? GetDefaultValue(ParameterInfo parameter) => parameter.HasDefaultValue ? $"({parameter.DefaultValue?.ToString()!.ToLower()})" : "";

        public static string ParameterToString(ParameterInfo parameterInfo) {
            return parameterInfo.Name!;
        }

        // TODO inherit from interface
        public string GetHelpString() {
            var result = "";

            result += "[DESCRIPTION]\n";
            result += ToStringHelper.GetMemberSummary(Method);
            result += "\n";

            result += "[POSITIONAL PARAMETERS]\n";
            foreach (var parameter in Parameters) {
                result += $"{ParameterToString(parameter)} # {ToStringHelper.GetTypeName(parameter.ParameterType)} {GetDefaultValue(parameter)}\n";
            }

            return result;
        }
    }
}