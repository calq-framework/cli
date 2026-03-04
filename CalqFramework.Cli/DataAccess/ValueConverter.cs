using System;
using System.Globalization;
using System.Reflection;
using CalqFramework.Extensions.System;

namespace CalqFramework.Cli.DataAccess;

/// <summary>
///     Converts values between CLI string representation and internal object types.
/// </summary>
public class ValueConverter : IValueConverter<string?> {
    /// <summary>
    ///     Culture to use for parsing values. Defaults to InvariantCulture.
    /// </summary>
    public IFormatProvider FormatProvider { get; init; } = CultureInfo.InvariantCulture;

    public bool CanConvert(Type targetType) => targetType.IsParsable() || targetType.IsEnum ||
                                               (Nullable.GetUnderlyingType(targetType)?.IsEnum ?? false);

    public string? ConvertFrom(object? value, Type targetType) => value?.ToString();

    public object? ConvertToOrUpdate(string? value, Type targetType, object? currentValue) {
        if (value == null) {
            return null;
        }

        Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        try {
            if (underlyingType.IsEnum) {
                return Enum.Parse(underlyingType, value, true);
            }

            return targetType.Parse(value, FormatProvider);
        } catch (ArgumentException ex) {
            throw CliErrors.InvalidValueFormat(targetType.Name, ex);
        } catch (OverflowException ex) {
            FieldInfo? minField = targetType.GetField("MinValue");
            FieldInfo? maxField = targetType.GetField("MaxValue");

            if (minField != null && maxField != null) {
                object? min = minField.GetValue(null);
                object? max = maxField.GetValue(null);
                throw CliErrors.ValueOutOfRange(min, max, ex);
            }

            throw;
        } catch (FormatException ex) {
            throw CliErrors.InvalidValueFormat(targetType.Name, ex);
        }
    }
}
