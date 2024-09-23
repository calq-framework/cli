using CalqFramework.Cli.Serialization;
using System;
using System.Collections;
using System.Linq;

namespace CalqFramework.Cli.Serialization.Parsing;
public static class ValueParser
{
    public static bool IsParseable(Type type)
    {
        return CalqFramework.Serialization.Text.ValueParser.IsParseable(type) || type.GetInterface(nameof(ICollection)) != null;
    }

    internal static object ParseValue(string value, Type type, string option)
    {
        try
        {
            var isCollection = type.GetInterface(nameof(ICollection)) != null;
            if (isCollection)
            {
                type = type.GetGenericArguments()[0];
            }

            var newValue = CalqFramework.Serialization.Text.ValueParser.ParseValue(value, type);
            return newValue;
        }
        catch (OverflowException ex)
        {
            throw new CliException($"option value is out of range: {option}={ex.Message}", ex);
        }
        catch (FormatException ex)
        {
            throw new CliException($"option and value type mismatch: {option}={value} ({option} is {type.Name})", ex);
        }
        catch (ArgumentException ex)
        {
            throw new CliException($"option and value type mismatch: {option}={value} ({option} is {type.Name})", ex);
        }
    }
}
