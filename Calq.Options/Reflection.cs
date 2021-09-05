using System;

namespace Calq.Options {
    internal static class Reflection {
        public static object? GetFieldOrPropertyValue(object obj, string fieldOrPropertyName) {
            var type = obj.GetType();
            var field = type.GetField(fieldOrPropertyName);
            if (field != null) {
                return field.GetValue(obj);
            } else {
                var property = type.GetProperty(fieldOrPropertyName);
                if (property != null) {
                    return property.GetValue(obj);
                }
            }
            throw new Exception($"option doesn't exist: {fieldOrPropertyName}"); // new MissingMemberException();
        }

        public static Type GetFieldOrPropertyType(Type type, string fieldOrPropertyName) {
            var field = type.GetField(fieldOrPropertyName);
            if (field != null) {
                return field.FieldType;
            } else {
                var property = type.GetProperty(fieldOrPropertyName);
                if (property != null) {
                    return property.PropertyType;
                }
            }
            throw new MissingMemberException();
        }

        public static void SetFieldOrPropertyValue(object obj, string fieldOrPropertyName, object? value) {
            var type = obj.GetType();
            var field = type.GetField(fieldOrPropertyName);
            if (field != null) {
                field.SetValue(obj, value);
            } else {
                var property = type.GetProperty(fieldOrPropertyName);
                if (property != null) {
                    property.SetValue(obj, value);
                } else {
                    throw new MissingMemberException();
                }
            }
        }

        public static object ParseValue(object obj, string fieldOrPropertyName, string value) {
            var currentValue = GetFieldOrPropertyValue(obj, fieldOrPropertyName);
            object newValue;
            try {
                switch (currentValue) {
                    case bool:
                        newValue = bool.Parse(value);
                        break;
                    case byte:
                        newValue = byte.Parse(value);
                        break;
                    case sbyte:
                        newValue = sbyte.Parse(value);
                        break;
                    case char:
                        newValue = char.Parse(value);
                        break;
                    case decimal:
                        newValue = decimal.Parse(value);
                        break;
                    case double:
                        newValue = double.Parse(value);
                        break;
                    case float:
                        newValue = float.Parse(value);
                        break;
                    case int:
                        newValue = int.Parse(value);
                        break;
                    case uint:
                        newValue = uint.Parse(value);
                        break;
                    case nint:
                        newValue = nint.Parse(value);
                        break;
                    case nuint:
                        newValue = nuint.Parse(value);
                        break;
                    case long:
                        newValue = long.Parse(value);
                        break;
                    case ulong:
                        newValue = ulong.Parse(value);
                        break;
                    case short:
                        newValue = short.Parse(value);
                        break;
                    case ushort:
                        newValue = ushort.Parse(value);
                        break;
                    default:
                        newValue = value; // assume string
                        break;
                }
            } catch (OverflowException ex) {
                long min;
                ulong max;
                switch (currentValue) {
                    case byte:
                        min = byte.MinValue;
                        max = byte.MaxValue;
                        break;
                    case sbyte:
                        min = sbyte.MinValue;
                        max = (ulong)sbyte.MaxValue;
                        break;
                    case char:
                        min = char.MinValue;
                        max = char.MaxValue;
                        break;
                    case int:
                        min = int.MinValue;
                        max = int.MaxValue;
                        break;
                    case uint:
                        min = uint.MinValue;
                        max = uint.MaxValue;
                        break;
                    case nint:
                        min = nint.MinValue;
                        max = (ulong)nint.MaxValue;
                        break;
                    case nuint:
                        min = (long)nuint.MinValue;
                        max = nuint.MaxValue;
                        break;
                    case long:
                        min = long.MinValue;
                        max = long.MaxValue;
                        break;
                    case ulong:
                        min = (long)ulong.MinValue;
                        max = ulong.MaxValue;
                        break;
                    case short:
                        min = short.MinValue;
                        max = (ulong)short.MaxValue;
                        break;
                    case ushort:
                        min = ushort.MinValue;
                        max = ushort.MaxValue;
                        break;
                    default:
                        throw;
                }
                throw new Exception($"option value is out of range: {fieldOrPropertyName}={value} ({min}-{max})", ex);
            } catch (FormatException ex) {
                var type = GetFieldOrPropertyType(obj.GetType(), fieldOrPropertyName);
                throw new Exception($"option and value type mismatch: {fieldOrPropertyName}={value} ({fieldOrPropertyName} is {type.Name})", ex);
            }

            return newValue;
        }
    }
}
