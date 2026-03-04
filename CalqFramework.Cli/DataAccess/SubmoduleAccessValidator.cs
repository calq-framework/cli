using System;
using System.Reflection;
using CalqFramework.Cli.Extensions.System.Reflection;
using CalqFramework.DataAccess.Extensions.System.Reflection;

namespace CalqFramework.Cli.DataAccess;

/// <summary>
///     Validates whether a member is a valid CLI submodule (must be non-convertible type).
/// </summary>
public sealed class SubmoduleAccessValidator(IValueConverter<string?> valueConverter) : IAccessValidator {
    private readonly IValueConverter<string?> _valueConverter = valueConverter;

    public bool IsValid(MemberInfo accessor) {
        Type type = accessor.GetUnderlyingType();
        return !_valueConverter.CanConvert(type);
    }
}
