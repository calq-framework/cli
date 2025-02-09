using CalqFramework.Cli.DataAccess;
using CalqFramework.Cli.DataAccess.InterfaceComponent;
using CalqFramework.Cli.InterfaceComponents;
using CalqFramework.Cli.Parsing;
using CalqFramework.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using static CalqFramework.Cli.Parsing.OptionReaderBase;

namespace CalqFramework.Cli {
    public class OptionDeserializer {
        public static void Deserialize(object obj, OptionDeserializerConfiguration? options = null) {
            Deserialize(obj, Environment.GetCommandLineArgs().Skip(1), options);
        }

        public static void Deserialize(object obj, IEnumerable<string> args, OptionDeserializerConfiguration? options = null) {
            var store = new CliComponentStoreFactory().CreateOptionStore(obj);
            Deserialize(store, args, options);
        }

        public static void Deserialize(IKeyValueStore<string, string> store, OptionDeserializerConfiguration? options = null) {
            Deserialize(store, Environment.GetCommandLineArgs().Skip(1), options);
        }

        public static void Deserialize(IKeyValueStore<string, string> store, IEnumerable<string> args, OptionDeserializerConfiguration? options = null) {
            using var argsEnumerator = args.GetEnumerator();
            var reader = new OptionReader(argsEnumerator, store);
            Deserialize(reader, options);
        }

        private static void Deserialize(OptionReader reader, OptionDeserializerConfiguration? options = null) {
            options ??= new OptionDeserializerConfiguration();

            var store = reader.Store;
            foreach (var (option, value, optionAttr) in reader.Read()) {
                if (optionAttr.HasFlag(OptionFlags.ValueUnassigned)) {
                    if (optionAttr.HasFlag(OptionFlags.Unknown)) {
                        if (options.SkipUnknown) {
                            continue;
                        }
                        throw new CliException($"not an option: {option}");
                    }
                    if (optionAttr.HasFlag(OptionFlags.AmbigousValue)) {
                        throw new CliException($"ambiguous syntax around {option} (try using --)");
                    }
                    throw new CliException($"unexpected value {option}");
                }

                store[option] = value;
            }
        }
    }
}
