using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CalqFramework.Cli.DataAccess.ClassMembers;

namespace CalqFramework.Cli.DataAccess.InterfaceComponents {
    /// <summary>
    /// Validates whether a method is a valid CLI subcommand (excludes .NET-specific methods).
    /// </summary>
    public class SubcommandAccessValidator : IAccessValidator {
        public bool IsValid(MemberInfo accessor) => !IsDotnetSpecific((MethodInfo)accessor);

        private static bool IsDotnetSpecific(MethodInfo methodInfo) {
            return methodInfo.DeclaringType == typeof(object) || methodInfo.GetCustomAttributes<CompilerGeneratedAttribute>().Any();
        }
    }
}
