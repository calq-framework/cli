using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CalqFramework.Cli.DataAccess.ClassMember {
    public class MethodResolver {
        public object Obj { get; }
        public BindingFlags BindingAttr { get; }
        public IEnumerable<MethodInfo> Methods { get => Obj.GetType().GetMethods(BindingAttr).Where(x => !IsDotnetSpecific(x)); }

        public MethodResolver(object obj, BindingFlags methodBindingAttr) {
            Obj = obj;
            BindingAttr = methodBindingAttr;
        }

        private static bool IsDotnetSpecific(MethodInfo methodInfo) {
            return methodInfo.DeclaringType == typeof(object) || methodInfo.GetCustomAttributes<CompilerGeneratedAttribute>().Any();
        }

        public MethodInfo GetMethod(string key) {
            var result = Obj.GetType().GetMethod(key, BindingAttr);
            if (result != null && !IsDotnetSpecific(result)) {
                return result;
            } else {
                throw new CliException($"invalid command");
            }
        }
    }
}