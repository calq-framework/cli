using CalqFramework.DataAccess;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.Parsing {
    internal class OptionsReader : OptionsReaderBase {
        private IKeyValueStore<string, object?> Store { get; }

        public OptionsReader(IEnumerator<string> argsEnumerator, IKeyValueStore<string, object?> dataStore) : base(argsEnumerator) {
            Store = dataStore;
        }

        protected override bool HasOption(char option) {
            return Store.ContainsKey(option.ToString());
        }

        protected override bool HasOption(string option) {
            return Store.ContainsKey(option);
        }

        protected override Type GetOptionType(char option) {
            return Store.GetDataType(option.ToString());
        }

        protected override Type GetOptionType(string option) {
            return Store.GetDataType(option);
        }
    }
}
