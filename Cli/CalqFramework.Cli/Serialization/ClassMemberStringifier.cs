using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.Serialization {

    public class ClassMemberStringifier : IClassMemberStringifier {

        public IEnumerable<string> GetNames(FieldInfo info) {
            return GetNames(info.Name, info.GetCustomAttributes<CliNameAttribute>());
        }

        public IEnumerable<string> GetNames(MethodInfo info) {
            return GetNames(info.Name, info.GetCustomAttributes<CliNameAttribute>());
        }

        public IEnumerable<string> GetNames(ParameterInfo info) {
            return GetNames(info.Name!, info.GetCustomAttributes<CliNameAttribute>());
        }

        public IEnumerable<string> GetNames(PropertyInfo info) {
            return GetNames(info.Name, info.GetCustomAttributes<CliNameAttribute>());
        }

        private IEnumerable<string> GetNames(string name, IEnumerable<CliNameAttribute> cliNameAttributes) {
            var keys = new List<string>();
            foreach (var cliNameAttribute in cliNameAttributes) {
                keys.Add(cliNameAttribute.Name);
            }
            if (!keys.Where(x => x.Length > 1).Any()) {
                keys.Add(name);
            }
            if (!keys.Where(x => x.Length == 1).Any()) {
                keys.Add(keys[0][0].ToString());
            }
            return keys;
        }
    }
}
