﻿using CalqFramework.Cli.DataAccess;
using CalqFramework.Cli.Serialization.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using static CalqFramework.Cli.Serialization.Parsing.OptionsReaderBase;

namespace CalqFramework.Cli {
    public class CliOptionsDeserializer {
        public static void Deserialize(object obj) {
            Deserialize(obj, new CliDeserializerOptions());
        }

        public static void Deserialize(object obj, CliDeserializerOptions options) {
            Deserialize(obj, options, Environment.GetCommandLineArgs().Skip(1));
        }

        public static void Deserialize(object obj, IEnumerable<string> args) {
            Deserialize(obj, new CliDeserializerOptions(), args);
        }

        public static void Deserialize(object obj, CliDeserializerOptions options, IEnumerable<string> args) {
            using var argsEnumerator = args.GetEnumerator();
            var store = options.CliOptionsStoreFactory.CreateDataMemberStore(obj);
            var reader = new OptionsReader(argsEnumerator, store);

            foreach (var (option, value, optionAttr) in reader.Read()) {
                if (optionAttr.HasFlag(OptionFlags.ValueUnassigned)) {
                    if (optionAttr.HasFlag(OptionFlags.Unknown)) {
                        if (options.SkipUnknown) {
                            continue;
                        }
                        throw new CliException($"unknown value {option}");
                    }
                    if (optionAttr.HasFlag(OptionFlags.AmbigousValue)) {
                        throw new CliException($"ambiguous syntax around {option} (try using --)");
                    }
                    throw new CliException($"unexpected value {option}");
                }

                var type = store.GetDataType(option);
                var valueObj = ValueParser.ParseValue(value, type, option);

                store[option] = valueObj;
            }
        }
    }
}
