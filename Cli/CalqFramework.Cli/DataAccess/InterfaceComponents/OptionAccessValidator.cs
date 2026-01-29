using System.Reflection;
using CalqFramework.Cli.DataAccess.ClassMembers;
using CalqFramework.DataAccess.Parsing;
using CalqFramework.Extensions.System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponents {

    /// <summary>
    /// Validates whether a member is a valid CLI option (must be parseable type).
    /// </summary>
    public class OptionAccessValidator : IAccessValidator {

        private readonly IStringParser _stringParser;

        public OptionAccessValidator(IStringParser stringParser) {
            _stringParser = stringParser;
        }

        public bool IsValid(MemberInfo accessor) {
            return _stringParser.IsParseable(accessor.GetUnderlyingType());
        }
    }
}
