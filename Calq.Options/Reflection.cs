using System;

namespace Ghbvft6.Calq.Options {
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
            throw new Exception($"option doesn't exist: {fieldOrPropertyName}"); // new MissingMemberException();
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

        private static object ParseValue(Type type, string value) {
            object objValue;
            try {
                switch (Type.GetTypeCode(type)) {
                    case TypeCode.Boolean:
                        objValue = bool.Parse(value);
                        break;
                    case TypeCode.Byte:
                        objValue = byte.Parse(value);
                        break;
                    case TypeCode.SByte:
                        objValue = sbyte.Parse(value);
                        break;
                    case TypeCode.Char:
                        objValue = char.Parse(value);
                        break;
                    case TypeCode.Decimal:
                        objValue = decimal.Parse(value);
                        break;
                    case TypeCode.Double:
                        objValue = double.Parse(value);
                        break;
                    case TypeCode.Single:
                        objValue = float.Parse(value);
                        break;
                    case TypeCode.Int32:
                        objValue = int.Parse(value);
                        break;
                    case TypeCode.UInt32:
                        objValue = uint.Parse(value);
                        break;
                    case TypeCode.Int64:
                        objValue = long.Parse(value);
                        break;
                    case TypeCode.UInt64:
                        objValue = ulong.Parse(value);
                        break;
                    case TypeCode.Int16:
                        objValue = short.Parse(value);
                        break;
                    case TypeCode.UInt16:
                        objValue = ushort.Parse(value);
                        break;
                    case TypeCode.String:
                        objValue = value;
                        break;
                    default:
                        throw new ArgumentException($"type cannot be parsed: {type.Name}");
                }
            } catch (OverflowException ex) {
                long min;
                ulong max;
                switch (Type.GetTypeCode(type)) {
                    case TypeCode.Byte:
                        min = byte.MinValue;
                        max = byte.MaxValue;
                        break;
                    case TypeCode.SByte:
                        min = sbyte.MinValue;
                        max = (ulong)sbyte.MaxValue;
                        break;
                    case TypeCode.Char:
                        min = char.MinValue;
                        max = char.MaxValue;
                        break;
                    case TypeCode.Int32:
                        min = int.MinValue;
                        max = int.MaxValue;
                        break;
                    case TypeCode.UInt32:
                        min = uint.MinValue;
                        max = uint.MaxValue;
                        break;
                    case TypeCode.Int64:
                        min = long.MinValue;
                        max = long.MaxValue;
                        break;
                    case TypeCode.UInt64:
                        min = (long)ulong.MinValue;
                        max = ulong.MaxValue;
                        break;
                    case TypeCode.Int16:
                        min = short.MinValue;
                        max = (ulong)short.MaxValue;
                        break;
                    case TypeCode.UInt16:
                        min = ushort.MinValue;
                        max = ushort.MaxValue;
                        break;
                    default:
                        throw;
                }
                throw new OverflowException($"{value} ({min}-{max})", ex);
            } catch (FormatException ex) {
                throw new FormatException($"value type mismatch: {value} is not {type.Name}", ex);
            }
            return objValue;
        }

        public static object ParseValue(Type type, string value, string fieldOrPropertyName) {
            object newValue;
            try {
                newValue = ParseValue(type, value);
            } catch (OverflowException ex) {
                throw new Exception($"option value is out of range: {fieldOrPropertyName}={ex.Message}", ex);
            } catch (FormatException ex) {
                throw new Exception($"option and value type mismatch: {fieldOrPropertyName}={value} ({fieldOrPropertyName} is {type.Name})", ex);
            } catch (ArgumentException ex) {
                throw new Exception($"option and value type mismatch: {fieldOrPropertyName}={value} ({fieldOrPropertyName} is {type.Name})", ex);
            }
            return newValue;
        }
    }
}
