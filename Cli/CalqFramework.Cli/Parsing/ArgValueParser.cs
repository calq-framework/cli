using System;
using System.Collections;
using CalqFramework.DataAccess.Parsing;

namespace CalqFramework.Cli.Parsing;

/// <summary>
/// Parses string values to typed objects in CLI context with enhanced error messages.
/// </summary>
public class ArgValueParser : IStringParser {
    private readonly StringParser _stringParser = new();

    public bool IsParsable(Type type) {
        return _stringParser.IsParsable(type) || type.GetInterface(nameof(ICollection)) != null;
    }

    public T ParseValue<T>(string value) {
        return (T)ParseValue(value, typeof(T));
    }

    public object ParseValue(string value, Type type) {
        bool isCollection = type.GetInterface(nameof(ICollection)) != null;
        if (isCollection) {
            type = type.GetGenericArguments()[0];
        }

        try {
            return _stringParser.ParseValue(value, type);
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
            throw new ArgValueParserException($"out of range ({min}-{max})", ex);
        } catch (FormatException ex) {
            throw new ArgValueParserException($"invalid format (expected {type.Name})", ex);
        }
    }
}
