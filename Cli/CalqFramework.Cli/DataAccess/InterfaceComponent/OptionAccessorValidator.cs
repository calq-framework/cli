using CalqFramework.Cli.DataAccess.ClassMember;
using CalqFramework.Cli.Parsing;
using CalqFramework.Extensions.System.Reflection;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    public class OptionAccessorValidator : IAccessorValidator {
        public bool IsValid(MemberInfo accessor) {
            return ValueParser.IsParseable(accessor.GetUnderlyingType());
        }
    }
}