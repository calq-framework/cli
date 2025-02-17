using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.Serialization {
    public interface IClassMemberStringifier {
        IEnumerable<string> GetAlternativeNames(FieldInfo info);
        IEnumerable<string> GetAlternativeNames(MethodInfo info);
        IEnumerable<string> GetAlternativeNames(ParameterInfo info);
        IEnumerable<string> GetAlternativeNames(PropertyInfo info);
        IEnumerable<string> GetRequiredNames(FieldInfo info);
        IEnumerable<string> GetRequiredNames(MethodInfo info);
        IEnumerable<string> GetRequiredNames(PropertyInfo info);
        IEnumerable<string> GetRequiredNames(ParameterInfo info);
    }
}
