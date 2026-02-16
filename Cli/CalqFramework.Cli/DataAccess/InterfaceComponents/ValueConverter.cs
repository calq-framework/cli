using System;
using CalqFramework.Cli.DataAccess.ClassMembers;
using CalqFramework.Extensions.System;

namespace CalqFramework.Cli.DataAccess.InterfaceComponents {

    /// <summary>
    /// Converts values between CLI string representation and internal object types.
    /// </summary>
    public class ValueConverter : IValueConverter<string?> {

        /// <summary>
        /// Culture to use for parsing values. Defaults to InvariantCulture.
        /// </summary>
        public IFormatProvider FormatProvider { get; init; } = System.Globalization.CultureInfo.InvariantCulture;

        public bool IsConvertible(Type type) {
            return type.IsParsable() || type.IsEnum || (Nullable.GetUnderlyingType(type)?.IsEnum ?? false);
        }

        public string? ConvertFromInternalValue(object? value, Type internalType) {
            return value?.ToString();
        }

        public object? ConvertToInternalValue(string? value, Type internalType, object? currentValue) {
            if (value == null) {
                return null;
            }

            var underlyingType = Nullable.GetUnderlyingType(internalType) ?? internalType;

            try {
                if (underlyingType.IsEnum) {
                    return Enum.Parse(underlyingType, value, ignoreCase: true);
                }

                return internalType.Parse(value, FormatProvider);
            } catch (ArgumentException ex) {
                throw CliErrors.InvalidValueFormat(internalType.Name, ex);
            } catch (OverflowException ex) {
                var minField = internalType.GetField("MinValue");
                var maxField = internalType.GetField("MaxValue");
                
                if (minField != null && maxField != null) {
                    var min = minField.GetValue(null);
                    var max = maxField.GetValue(null);
                    throw CliErrors.ValueOutOfRange(min, max, ex);
                }
                throw;
            } catch (FormatException ex) {
                throw CliErrors.InvalidValueFormat(internalType.Name, ex);
            }
        }
    }
}
