using System;

namespace CalqFramework.Options;
public static class ValueParser
{
    public static object ParseValue(string value, Type type, string option) {
        try {
            var newValue = Serialization.Text.ValueParser.ParseValue(value, type);
            return newValue;
        } catch (OverflowException ex) {
            throw new Exception($"option value is out of range: {option}={ex.Message}", ex);
        } catch (FormatException ex) {
            throw new Exception($"option and value type mismatch: {option}={value} ({option} is {type.Name})", ex);
        } catch (ArgumentException ex) {
            throw new Exception($"option and value type mismatch: {option}={value} ({option} is {type.Name})", ex);
        }
    }
}
