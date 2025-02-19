using System;
using System.Collections;

namespace CalqFramework.Cli.Parsing;

// TODO convert to a regular class
// TODO DRY
public static class ValueParser {

    public static bool IsParseable(Type type) {
        return CalqFramework.DataAccess.Text.ValueParser.IsParseable(type) || type.GetInterface(nameof(ICollection)) != null;
    }

    internal static object ParseValue(string value, Type type) {
        try {
            bool isCollection = type.GetInterface(nameof(ICollection)) != null;
            if (isCollection) {
                type = type.GetGenericArguments()[0];
            }

            object newValue = CalqFramework.DataAccess.Text.ValueParser.ParseValue(value, type);
            return newValue;
        } catch (OverflowException ex) {
            throw new CliException($"value is out of range: {ex.Message}", ex);
        } catch (FormatException ex) {
            throw new CliException($"value type mismatch: expected {type.Name} got {value}", ex);
        } catch (ArgumentException ex) {
            throw new CliException($"value type mismatch: expected {type.Name} got {value})", ex);
        }
    }

    internal static object ParseValue(string value, Type type, string option) {
        try {
            bool isCollection = type.GetInterface(nameof(ICollection)) != null;
            if (isCollection) {
                type = type.GetGenericArguments()[0];
            }

            object newValue = CalqFramework.DataAccess.Text.ValueParser.ParseValue(value, type);
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
