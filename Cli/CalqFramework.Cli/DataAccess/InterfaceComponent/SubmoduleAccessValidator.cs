using System.Reflection;
using CalqFramework.Cli.DataAccess.ClassMember;
using CalqFramework.Cli.Parsing;
using CalqFramework.Extensions.System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {

    /// <summary>
    /// Validates whether a member is a valid CLI submodule (must be non-parseable type).
    /// </summary>
    public class SubmoduleAccessValidator : IAccessValidator {

        public bool IsValid(MemberInfo accessor) {
            return !ValueParser.IsParseable(accessor.GetUnderlyingType());
        }
    }
}
