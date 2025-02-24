using System.Reflection;
using CalqFramework.Cli.DataAccess.ClassMember;
using CalqFramework.Cli.Parsing;
using CalqFramework.Extensions.System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {

    public class OptionAccessValidator : IAccessValidator {

        public bool IsValid(MemberInfo accessor) {
            return ValueParser.IsParseable(accessor.GetUnderlyingType());
        }
    }
}
