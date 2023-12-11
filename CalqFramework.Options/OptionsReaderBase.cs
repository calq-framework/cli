using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace CalqFramework.Options {
    public abstract class OptionsReaderBase {

        [Flags]
        public enum OptionFlags {
            None = 0,
            Short = 1,
            Plus = 2,
            ValueUnassigned = 4,
            NotAnOption = 8 + ValueUnassigned,
            Unknown = 16 + ValueUnassigned,
            AmbigousValue = 32 + ValueUnassigned // ambiguous value (starts with '-')
        }

        public int LastIndex { get; private set; }

        protected abstract bool ValidateOptionName(char option);
        protected abstract bool TryGetOptionType(string option, out Type result);

        public IEnumerable<(string option, string value, OptionFlags optionAttr)> Read(string[] args, int startIndex = 0) {

            bool IsNumber(string input) {
                return BigInteger.TryParse(input, out _);
            }

            void ValidateOptionValue(string option, ref string value, ref OptionFlags optionAttr, ref int index) {
                if (TryGetOptionType(option, out Type type)) {
                    if (value == "") {
                        var isCollection = type.GetInterface(nameof(ICollection)) != null;
                        if (isCollection) {
                            type = type.GetGenericArguments()[0];
                        }
                        if (type == typeof(bool)) {
                            value = optionAttr.HasFlag(OptionFlags.Plus) ? "false" : "true";
                        } else {
                            if (index + 1 < args.Length && (args[index + 1][0] != '-' || IsNumber(args[index + 1]))) { // fail if starts with '-' to prevent human error
                                ++index;
                                value = args[index];
                            } else {
                                optionAttr |= OptionFlags.ValueUnassigned;
                            }
                        }
                    }
                } else {
                    optionAttr |= OptionFlags.Unknown;
                }
            }

            (string option, string value) ExtractOptionValuePair(string arg, OptionFlags optionAttr) {
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

            IEnumerable<char> ReadShort(string stackedOptions) {
                for (var i = 0; i < stackedOptions.Length; ++i) {
                    var option = stackedOptions[i];
                    yield return option;
                }
            }

            int index = startIndex;

            try {
                while (true) {
                    if (index >= args.Length) {
                        yield break;
                    }

                    var arg = args[index];

                    if (arg.Length == 0) {
                        throw new ArgumentException("arg length is 0");
                    }

                    var optionAttr = OptionFlags.None;
                    switch (arg[0]) {
                        case '-':
                            if (arg[1] != '-') {
                                optionAttr |= OptionFlags.Short;
                            } else {
                                if (arg.Length == 2) {
                                    ++index;
                                    yield break;
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
                            ++index;
                            optionAttr |= OptionFlags.NotAnOption;
                            yield return (arg, "", optionAttr);
                            continue;
                    }

                    var (option, value) = ExtractOptionValuePair(arg, optionAttr);
                    if (value == "") {
                        if (index + 1 < args.Length && (args[index + 1][0] != '-' || IsNumber(args[index + 1]))) {
                            ++index;
                            value = args[index];
                        }
                    }

                    if (optionAttr.HasFlag(OptionFlags.Short)) {
                        foreach (var shortOption in ReadShort(option)) {
                            if (ValidateOptionName(shortOption)) {
                                ValidateOptionValue(shortOption.ToString(), ref value, ref optionAttr, ref index);
                                yield return (shortOption.ToString(), value, optionAttr);
                            } else {
                                optionAttr |= OptionFlags.Unknown;
                                yield return (shortOption.ToString(), value, optionAttr);
                            }
                        }
                    } else {
                        ValidateOptionValue(option, ref value, ref optionAttr, ref index);
                        yield return (option, value, optionAttr);
                    }

                    ++index;
                }
            } finally {
                LastIndex = index;
            }
        }
    }
}
