using System;
using CalqFramework.Extensions.System;

namespace CalqFramework.Cli.DataAccess {

    /// <summary>
    /// Converts values between CLI string representation and internal object types.
    /// </summary>
    public class ValueConverter : IValueConverter<string?> {

        /// <summary>
        /// Culture to use for parsing values. Defaults to InvariantCulture.
        /// </summary>
        public IFormatProvider FormatProvider { get; init; } = System.Globalization.CultureInfo.InvariantCulture;

        public bool CanConvert(Type targetType) {
            return targetType.IsParsable() || targetType.IsEnum || (Nullable.GetUnderlyingType(targetType)?.IsEnum ?? false);
        }

        public string? ConvertFrom(object? value, Type targetType) {
            return value?.ToString();
        }

        public object? ConvertToOrUpdate(string? value, Type targetType, object? currentValue) {
            if (value == null) {
                return null;
            }

            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            try {
                if (underlyingType.IsEnum) {
                    return Enum.Parse(underlyingType, value, ignoreCase: true);
                }

                return targetType.Parse(value, FormatProvider);
            } catch (ArgumentException ex) {
                throw CliErrors.InvalidValueFormat(targetType.Name, ex);
            } catch (OverflowException ex) {
                var minField = targetType.GetField("MinValue");
                var maxField = targetType.GetField("MaxValue");
                
                if (minField != null && maxField != null) {
                    var min = minField.GetValue(null);
                    var max = maxField.GetValue(null);
                    throw CliErrors.ValueOutOfRange(min, max, ex);
                }
                throw;
            } catch (FormatException ex) {
                throw CliErrors.InvalidValueFormat(targetType.Name, ex);
            }
        }
    }
}
