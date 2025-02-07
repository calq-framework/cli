using CalqFramework.Cli.Parsing;
using CalqFramework.Extensions.System.Reflection;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.ClassMember {
    public class CliCommandValidator : ICliValidator {
        public bool IsValid(MemberInfo accessor) {
            return !ValueParser.IsParseable(accessor.GetUnderlyingType());
        }
    }
}