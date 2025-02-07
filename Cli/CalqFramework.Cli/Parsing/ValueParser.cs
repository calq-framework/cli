using System;
using System.Collections;

namespace CalqFramework.Cli.Parsing;

// TODO convert to a regular class
public static class ValueParser {
    public static bool IsParseable(Type type) {
        return CalqFramework.DataAccess.Text.ValueParser.IsParseable(type) || type.GetInterface(nameof(ICollection)) != null;
    }

    internal static object ParseValue(string value, Type type, string option) {
        try {
            var isCollection = type.GetInterface(nameof(ICollection)) != null;
            if (isCollection) {
                type = type.GetGenericArguments()[0];
            }

            var newValue = CalqFramework.DataAccess.Text.ValueParser.ParseValue(value, type);
            return newValue;
        } catch (OverflowException ex) {
            throw new CliException($"option value is out of range: {option}={ex.Message}", ex);
        } catch (FormatException ex) {
            throw new CliException($"option and value type mismatch: {option}={value} ({option} is {type.Name})", ex);
        } catch (ArgumentException ex) {
            throw new CliException($"option and value type mismatch: {option}={value} ({option} is {type.Name})", ex);
        }
    }
}
