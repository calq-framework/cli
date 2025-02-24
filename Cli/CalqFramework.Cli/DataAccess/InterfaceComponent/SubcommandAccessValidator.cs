using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CalqFramework.Cli.DataAccess.ClassMember;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    public class SubcommandAccessValidator : IAccessValidator {
        public bool IsValid(MemberInfo accessor) => !IsDotnetSpecific((MethodInfo)accessor);

        private static bool IsDotnetSpecific(MethodInfo methodInfo) {
            return methodInfo.DeclaringType == typeof(object) || methodInfo.GetCustomAttributes<CompilerGeneratedAttribute>().Any();
        }
    }
}
