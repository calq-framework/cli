using System.Reflection;
using CalqFramework.Cli.DataAccess.ClassMembers;
using CalqFramework.Cli.Parsing;
using CalqFramework.Extensions.System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponents {

    /// <summary>
    /// Validates whether a member is a valid CLI submodule (must be non-parsable type).
    /// </summary>
    public class SubmoduleAccessValidator : IAccessValidator {

        private readonly IArgValueParser _argValueParser;

        public SubmoduleAccessValidator(IArgValueParser argValueParser) {
            _argValueParser = argValueParser;
        }

        public bool IsValid(MemberInfo accessor) {
            return !_argValueParser.IsParsable(accessor.GetUnderlyingType());
        }
    }
}
