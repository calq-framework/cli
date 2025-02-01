using CalqFramework.Cli.Serialization.Parsing;
using CalqFramework.Extensions.System.Reflection;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.ClassMember {
    public class CliOptionValidator : ICliValidator {
        public bool IsValid(MemberInfo accessor) {
            return ValueParser.IsParseable(accessor.GetUnderlyingType());
        }
    }
}