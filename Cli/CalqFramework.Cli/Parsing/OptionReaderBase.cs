using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace CalqFramework.Cli.Parsing {

    internal abstract class OptionReaderBase {

        protected OptionReaderBase(IEnumerator<string> argsEnumerator) {
            ArgsEnumerator = argsEnumerator;
        }

        [Flags]
        internal enum OptionFlags {
            None = 0,
            Short = 1,
            Plus = 2,
            ValueUnassigned = 4,
            NotAnOption = 8 + ValueUnassigned,
            Unknown = 16 + ValueUnassigned,
            AmbigousValue = 32 + ValueUnassigned // ambiguous value (starts with '-')
        }

        public IEnumerator<string> ArgsEnumerator { get; }

        public IEnumerable<(string option, string value, OptionFlags optionAttr)> Read() {
            bool IsNumber(string input) {
                return BigInteger.TryParse(input, out _);
            }

            void TrySelfAssign(Type type, ref string value, ref OptionFlags optionAttr) {
                var isCollection = type.GetInterface(nameof(ICollection)) != null;
                if (isCollection) {
                    type = type.GetGenericArguments()[0];
                }
                if (type == typeof(bool)) {
                    value = optionAttr.HasFlag(OptionFlags.Plus) ? "false" : "true";
                    optionAttr ^= OptionFlags.AmbigousValue; // assume OptionFlags.AmbigousValue is set, assume AmbigousValue = 32 + ValueUnassigned
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

            var moved = false;
            while (moved || ArgsEnumerator.MoveNext()) {
                moved = false;
                var arg = ArgsEnumerator.Current;

                if (arg.Length == 0) {
                    throw new ArgumentException("arg length is 0");
                }

                var optionAttr = OptionFlags.None;
                switch (arg[0]) {
                    case '-':
                        if (arg[1] == '-') {
                            if (arg.Length == 2) {
                                yield break;
                            }
                        } else {
                            optionAttr |= OptionFlags.Short;
                        }
                        break;

                    case '+':
                        optionAttr |= OptionFlags.Plus;
                        if (arg[1] != '+') {
                            optionAttr |= OptionFlags.Short;
                        }
                        break;

                    default:
                        optionAttr |= OptionFlags.NotAnOption;
                        yield return (arg, "", optionAttr);
                        continue;
                }

                var (option, value) = ExtractOptionValuePair(arg, optionAttr);
                if (value == "") {
                    moved = ArgsEnumerator.MoveNext();
                    if (moved) {
                        if (ArgsEnumerator.Current[0] != '-' || IsNumber(ArgsEnumerator.Current)) {
                            value = ArgsEnumerator.Current;
                            moved = false;
                        } else {
                            optionAttr |= OptionFlags.AmbigousValue; // fail if starts with '-' to prevent human error
                        }
                    } else {
                        optionAttr |= OptionFlags.ValueUnassigned;
                    }
                }

                if (optionAttr.HasFlag(OptionFlags.Short)) {
                    foreach (var shortOption in ReadShort(option)) {
                        if (HasOption(shortOption)) {
                            if (optionAttr.HasFlag(OptionFlags.ValueUnassigned)) {
                                TrySelfAssign(GetOptionType(shortOption), ref value, ref optionAttr);
                            }
                        } else {
                            optionAttr |= OptionFlags.Unknown;
                        }
                        yield return (shortOption.ToString(), value, optionAttr);
                    }
                } else {
                    if (HasOption(option)) {
                        if (optionAttr.HasFlag(OptionFlags.ValueUnassigned)) {
                            TrySelfAssign(GetOptionType(option), ref value, ref optionAttr);
                        }
                    } else {
                        optionAttr |= OptionFlags.Unknown;
                    }
                    yield return (option, value, optionAttr);
                }
            }
        }

        protected abstract Type GetOptionType(char option);

        protected abstract Type GetOptionType(string option);

        protected abstract bool HasOption(char option);

        protected abstract bool HasOption(string option);
    }
}