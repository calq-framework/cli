using System;

namespace Calq.Options {
    internal class Reader {

        private string[] args;
        private int index;
        private object instance;

        public Reader(string[] args, object instance) {
            this.args = args;
            this.instance = instance;
        }

        [Flags]
        private enum OptionFlags {
            None = 0,
            Short = 1,
            Plus = 2
        }

        private string GetOptionName(char option) {
            foreach (var field in instance.GetType().GetFields()) {
                if (field.Name[0] == option) {
                    return field.Name;
                }
            }
            foreach (var property in instance.GetType().GetProperties()) {
                if (property.Name[0] == option) {
                    return property.Name;
                }
            }
            throw new Exception($"option doesn't exist: {option}");
        }

        private static object? GetFieldOrPropertyValue(object obj, string fieldOrPropertyName) {
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
            throw new Exception($"option doesn't exist: {fieldOrPropertyName}");
        }

        private static Type GetFieldOrPropertyType(object obj, string fieldOrPropertyName) {
            var type = obj.GetType();
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

        private static void SetFieldOrPropertyValue(object obj, string fieldOrPropertyName, object? value) {
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

        private bool IsBoolean(string option) {
            return GetFieldOrPropertyType(instance, option) == typeof(bool);
        }

        private void SetOption(string option, string value) {
            var currentValue = GetFieldOrPropertyValue(instance, option);
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
                throw new Exception($"option value is out of range: {option}={value} ({min}-{max})", ex);
            } catch (FormatException ex) {
                var type = GetFieldOrPropertyType(instance, option);
                throw new Exception($"option and value type mismatch: {option}={value} ({option} is {type.Name})", ex);
            }
            try {
                SetFieldOrPropertyValue(instance, option, newValue);
            } catch (ArgumentException ex) {
                var type = GetFieldOrPropertyType(instance, option);
                throw new Exception($"option and value type mismatch: {option}={value} ({option} is {type.Name})", ex);
            }
        }

        public void Read() {
            while (ReadOption()) { }
        }

        private bool ReadOption() {
            if (index >= args.Length) {
                return false;
            }

            var arg = args[index];

            if (arg.Length == 0) {
                throw new ArgumentException("arg length is 0");
            }

            if (arg.Length == 1) {
                return false;
            }

            var optionAttr = OptionFlags.None;
            switch (arg[0]) {
                case '-':
                    if (arg[1] != '-') {
                        optionAttr |= OptionFlags.Short;
                    } else {
                        if (arg.Length == 2) {
                            ++index;
                            return false;
                        }
                    }
                    break;
                case '+':
                    optionAttr |= OptionFlags.Plus;
                    if (arg[1] != '+') {
                        optionAttr |= OptionFlags.Short;
                    }
                    break;
                default:
                    return false;
            }

            var (option, value) = ExtractOptionValuePair(arg, optionAttr);
            if (value == "") {
                if (IsBoolean(optionAttr.HasFlag(OptionFlags.Short) ? GetOptionName(option[0]) : option)) {
                    value = optionAttr.HasFlag(OptionFlags.Plus) ? "false" : "true";
                } else {
                    ++index;
                    try {
                        value = args[index];
                    } catch (IndexOutOfRangeException ex) {
                        throw new Exception($"option requires value: {arg}", ex);
                    }
                }
            }

            if (optionAttr.HasFlag(OptionFlags.Short)) {
                ReadShort(option, value);
            } else {
                SetOption(option, value);
            }

            ++index;
            return true;
        }

        private (string option, string value) ExtractOptionValuePair(string arg, OptionFlags optionAttr) {
            var optionValueSplit = arg.Split('=', 2);
            string value = optionValueSplit.Length == 1 ? "" : optionValueSplit[1];

            string option;
            if (optionAttr.HasFlag(OptionFlags.Short)) {
                option = optionValueSplit[0][1..];
            } else {
                option = optionValueSplit[0][2..];
            }

            return (option, value);
        }

        private void ReadShort(string stackedOptions, string value) {
            var option = GetOptionName(stackedOptions[0]);
            if (stackedOptions.Length > 1 && IsBoolean(option) == false) {
                throw new Exception($"not all stacked options are boolean: {stackedOptions}");
            }

            SetOption(option, value);

            for (var i = 1; i < stackedOptions.Length; ++i) {
                option = GetOptionName(stackedOptions[i]);
                if (IsBoolean(option) == false) {
                    throw new Exception($"not all stacked options are boolean: {stackedOptions}");
                }
                SetOption(option, value);
            }
        }
    }
}
