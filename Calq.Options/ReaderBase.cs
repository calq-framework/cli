using System;
using System.Collections.Generic;

namespace Ghbvft6.Calq.Options {
    public abstract class ReaderBase {

        [Flags]
        private enum OptionFlags {
            None = 0,
            Short = 1,
            Plus = 2
        }

        public int LastIndex { get; private set; }

        protected abstract string GetOptionName(char option);
        protected abstract Type GetOptionType(string option);

        public IEnumerable<(string option, string value)> Read(string[] args, int startIndex = 0) {

            bool IsBoolean(string option) {
                return GetOptionType(option) == typeof(bool);
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

            IEnumerable<string> ReadShort(string stackedOptions) {
                var option = GetOptionName(stackedOptions[0]);
                if (stackedOptions.Length > 1 && IsBoolean(option) == false) {
                    throw new Exception($"not all stacked options are boolean: {stackedOptions}");
                }

                yield return option;

                for (var i = 1; i < stackedOptions.Length; ++i) {
                    option = GetOptionName(stackedOptions[i]);
                    if (IsBoolean(option) == false) {
                        throw new Exception($"not all stacked options are boolean: {stackedOptions}");
                    }
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

                    if (arg.Length == 1) {
                        yield break;
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
                            yield break;
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
                        foreach (var longOption in ReadShort(option)) {
                            yield return (longOption, value);
                        }
                    } else {
                        yield return (option, value);
                    }

                    ++index;
                }
            } finally {
                LastIndex = index;
            }
        }
    }
}
