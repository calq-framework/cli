using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.Serialization {
    public interface IClassMemberStringifier {
        IEnumerable<string> GetNames(FieldInfo info);
        IEnumerable<string> GetNames(MethodInfo info);
        IEnumerable<string> GetNames(ParameterInfo info);
        IEnumerable<string> GetNames(PropertyInfo info);
    }
}
