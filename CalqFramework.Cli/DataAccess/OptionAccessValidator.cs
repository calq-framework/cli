using System.Reflection;
using CalqFramework.Extensions.System;
using CalqFramework.Extensions.System.Reflection;

namespace CalqFramework.Cli.DataAccess {

    /// <summary>
    /// Validates whether a member is a valid CLI option (must be convertible type).
    /// </summary>
    public class OptionAccessValidator : IAccessValidator {

        private readonly IValueConverter<string?> _valueConverter;

        public OptionAccessValidator(IValueConverter<string?> valueConverter) {
            _valueConverter = valueConverter;
        }

        public bool IsValid(MemberInfo accessor) {
            var type = accessor.GetUnderlyingType();
            return _valueConverter.CanConvert(type);
        }
    }
}
