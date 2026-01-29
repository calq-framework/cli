using System.Reflection;
using CalqFramework.Cli.DataAccess.ClassMembers;
using CalqFramework.DataAccess.Parsing;
using CalqFramework.Extensions.System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponents {

    /// <summary>
    /// Validates whether a member is a valid CLI submodule (must be non-parseable type).
    /// </summary>
    public class SubmoduleAccessValidator : IAccessValidator {

        private readonly IStringParser _stringParser;

        public SubmoduleAccessValidator(IStringParser stringParser) {
            _stringParser = stringParser;
        }

        public bool IsValid(MemberInfo accessor) {
            return !_stringParser.IsParseable(accessor.GetUnderlyingType());
        }
    }
}
