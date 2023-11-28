using System;
using System.Collections.Generic;
using System.Numerics;

namespace CalqFramework.Options {
    public abstract class OptionsReaderBase {

        [Flags]
        public enum OptionFlags {
            None = 0,
            Short = 1,
            Plus = 2,
            NotAnOption = 4
        }

        public int LastIndex { get; private set; }

        protected abstract string GetOptionName(char option);
        protected abstract Type GetOptionType(string option);

        public IEnumerable<(string option, string value, OptionFlags optionAttr)> Read(string[] args, int startIndex = 0, bool skipUnknown = false) {

            bool IsBoolean(string option) {
                return GetOptionType(option) == typeof(bool);
            }

            bool IsNumber(string input) {
                return BigInteger.TryParse(input, out _);
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
                //var option = GetOptionName(stackedOptions[0]);
                //if (stackedOptions.Length > 1 && IsBoolean(option) == false) {
                //    throw new Exception($"not all stacked options are boolean: {stackedOptions}");
                //}

                //yield return option;

                for (var i = 0; i < stackedOptions.Length; ++i) {
                    var option = stackedOptions[i].ToString();
                    //option = GetOptionName(stackedOptions[i]);
                    //if (IsBoolean(option) == false) {
                    //    throw new Exception($"not all stacked options are boolean: {stackedOptions}");
                    //}
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
                            yield return (arg, "", OptionFlags.NotAnOption);
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
                        foreach (var longOption in ReadShort(option)) {
                            yield return (longOption, value, optionAttr);
                        }
                    } else {
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
