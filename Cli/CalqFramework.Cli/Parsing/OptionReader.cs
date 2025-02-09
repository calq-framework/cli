﻿using CalqFramework.DataAccess;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.Parsing {
    internal class OptionReader : OptionReaderBase {
        public IKeyValueStore<string, string?> Store { get; }

        public OptionReader(IEnumerator<string> argsEnumerator, IKeyValueStore<string, string?> store) : base(argsEnumerator) {
            Store = store;
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
