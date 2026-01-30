using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using CalqFramework.DataAccess;

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
            AmbigousValue = 32 + ValueUnassigned // ambiguous value (starts with '-' or '+')
        }

        public IEnumerator<string> ArgsEnumerator { get; }

        public IEnumerable<(string option, string value, OptionFlags optionAttr)> Read() {
            bool IsNumber(string input) {
                return BigInteger.TryParse(input, out _);
            }

            void TrySelfAssign(Type type, ref string value, ref OptionFlags optionAttr) {
                bool isCollection = type.GetInterface(nameof(ICollection)) != null;
                if (isCollection) {
                    type = type.GetGenericArguments()[0];
                }
                if (type == typeof(bool)) {
                    value = optionAttr.HasFlag(OptionFlags.Plus) ? "false" : "true";
                    optionAttr |= OptionFlags.AmbigousValue;
                    optionAttr ^= OptionFlags.AmbigousValue;
                }
            }

            (string option, string value, bool assigned) ExtractOptionValuePair(string arg, OptionFlags optionAttr) {
                string[] optionValueSplit = arg.Split('=', 2);
                string value = optionValueSplit.Length == 1 ? "" : optionValueSplit[1];
                bool assigned = optionValueSplit.Length != 1;

                string option;
                if (optionAttr.HasFlag(OptionFlags.Short)) {
                    option = optionValueSplit[0][1..];
                } else {
                    option = optionValueSplit[0][2..];
                }

                return (option, value, assigned);
            }

            IEnumerable<char> ReadShort(string stackedOptions) {
                for (int i = 0; i < stackedOptions.Length; ++i) {
                    char option = stackedOptions[i];
                    yield return option;
                }
            }

            bool moved = false;
            while (moved || ArgsEnumerator.MoveNext()) {
                moved = false;
                string arg = ArgsEnumerator.Current;

                if (arg.Length == 0) {
                    throw new ArgumentException("Arg length is 0");
                }

                OptionFlags optionAttr = OptionFlags.None;
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

                (string option, string value, bool assigned) = ExtractOptionValuePair(arg, optionAttr);
                if (!assigned) {
                    moved = ArgsEnumerator.MoveNext();
                    if (moved) {
                        if (ArgsEnumerator.Current == "" || (ArgsEnumerator.Current[0] != '-' && ArgsEnumerator.Current[0] != '+') || IsNumber(ArgsEnumerator.Current)) {
                            value = ArgsEnumerator.Current;
                            moved = false;
                        } else {
                            optionAttr |= OptionFlags.AmbigousValue;
                        }
                    } else {
                        optionAttr |= OptionFlags.ValueUnassigned;
                    }
                }

                if (optionAttr.HasFlag(OptionFlags.Short)) {
                    foreach (char shortOption in ReadShort(option)) {
                        try {
                            if (HasOption(shortOption)) {
                                if (optionAttr.HasFlag(OptionFlags.ValueUnassigned)) {
                                    TrySelfAssign(GetOptionType(shortOption), ref value, ref optionAttr);
                                }
                            } else {
                                optionAttr |= OptionFlags.Unknown;
                            }
                        } catch (ArgValueParserException ex) {
                            throw CliErrors.OptionError(shortOption.ToString(), ex.Message, ex);
                        } catch (DataAccessException ex) {
                            throw CliErrors.OptionError(shortOption.ToString(), ex.Message, ex);
                        }
                        yield return (shortOption.ToString(), value, optionAttr);
                    }
                } else {
                    try {
                        if (HasOption(option)) {
                            if (optionAttr.HasFlag(OptionFlags.ValueUnassigned)) {
                                TrySelfAssign(GetOptionType(option), ref value, ref optionAttr);
                            }
                        } else {
                            optionAttr |= OptionFlags.Unknown;
                        }
                    } catch (ArgValueParserException ex) {
                        throw CliErrors.OptionError(option, ex.Message, ex);
                    } catch (DataAccessException ex) {
                        throw CliErrors.OptionError(option, ex.Message, ex);
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
