using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.Serialization {

    public abstract class ClassMemberStringifierBase : IClassMemberStringifier {
        public IEnumerable<string> GetAlternativeNames(FieldInfo info) {
            return GetAlternativeNames(info.Name, info.GetCustomAttributes<CliNameAttribute>());
        }

        public IEnumerable<string> GetAlternativeNames(MethodInfo info) {
            return GetAlternativeNames(info.Name, info.GetCustomAttributes<CliNameAttribute>());
        }

        public IEnumerable<string> GetAlternativeNames(ParameterInfo info) {
            return GetAlternativeNames(info.Name!, info.GetCustomAttributes<CliNameAttribute>());
        }

        public IEnumerable<string> GetAlternativeNames(PropertyInfo info) {
            return GetAlternativeNames(info.Name, info.GetCustomAttributes<CliNameAttribute>());
        }

        public IEnumerable<string> GetRequiredNames(FieldInfo info) {
            return GetRequiredNames(info.Name, info.GetCustomAttributes<CliNameAttribute>());
        }

        public IEnumerable<string> GetRequiredNames(MethodInfo info) {
            return GetRequiredNames(info.Name, info.GetCustomAttributes<CliNameAttribute>());
        }

        public IEnumerable<string> GetRequiredNames(PropertyInfo info) {
            return GetRequiredNames(info.Name, info.GetCustomAttributes<CliNameAttribute>());
        }

        public IEnumerable<string> GetRequiredNames(ParameterInfo info) {
            return GetRequiredNames(info.Name!, info.GetCustomAttributes<CliNameAttribute>());
        }
        protected abstract IEnumerable<string> GetAlternativeNames(string name, IEnumerable<CliNameAttribute> cliNameAttributes);

        protected abstract IEnumerable<string> GetRequiredNames(string name, IEnumerable<CliNameAttribute> cliNameAttributes);
    }
}
