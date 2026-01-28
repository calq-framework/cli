using System.Reflection;
using CalqFramework.Cli.DataAccess.ClassMembers;
using CalqFramework.DataAccess.Parsing;
using CalqFramework.Extensions.System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponents {

    /// <summary>
    /// Validates whether a member is a valid CLI submodule (must be non-parseable type).
    /// </summary>
    public class SubmoduleAccessValidator : IAccessValidator {

        private readonly IValueParser _valueParser;

        public SubmoduleAccessValidator(IValueParser valueParser) {
            _valueParser = valueParser;
        }

        public bool IsValid(MemberInfo accessor) {
            return !_valueParser.IsParseable(accessor.GetUnderlyingType());
        }
    }
}
