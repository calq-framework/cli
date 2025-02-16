using CalqFramework.DataAccess;
using System;
using System.Collections.Generic;

namespace CalqFramework.Cli.Parsing {

    internal class OptionReader : OptionReaderBase {

        public OptionReader(IEnumerator<string> argsEnumerator, IKeyValueStore<string, string?> store) : base(argsEnumerator) {
            Store = store;
        }

        public IKeyValueStore<string, string?> Store { get; }

        protected override Type GetOptionType(char option) {
            return Store.GetDataType(option.ToString());
        }

        protected override Type GetOptionType(string option) {
            return Store.GetDataType(option);
        }

        protected override bool HasOption(char option) {
            return Store.ContainsKey(option.ToString());
        }

        protected override bool HasOption(string option) {
            return Store.ContainsKey(option);
        }
    }
}