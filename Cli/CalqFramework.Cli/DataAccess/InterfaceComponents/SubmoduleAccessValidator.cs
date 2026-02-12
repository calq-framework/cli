using System.Reflection;
using CalqFramework.Cli.DataAccess.ClassMembers;
using CalqFramework.Extensions.System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponents {

    /// <summary>
    /// Validates whether a member is a valid CLI submodule (must be non-convertible type).
    /// </summary>
    public class SubmoduleAccessValidator : IAccessValidator {

        private readonly IValueConverter<string?> _valueConverter;

        public SubmoduleAccessValidator(IValueConverter<string?> valueConverter) {
            _valueConverter = valueConverter;
        }

        public bool IsValid(MemberInfo accessor) {
            return !_valueConverter.IsConvertible(accessor.GetUnderlyingType());
        }
    }
}
