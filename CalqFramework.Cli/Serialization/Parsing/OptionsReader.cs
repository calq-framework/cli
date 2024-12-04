using CalqFramework.Serialization.DataAccess;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.Serialization.Parsing
{
    internal class OptionsReader : OptionsReaderBase
    {
        private IKeyValueStore<string, object?> DataStore { get; }

        public OptionsReader(IEnumerator<string> argsEnumerator, IKeyValueStore<string, object?> dataStore) : base(argsEnumerator)
        {
            DataStore = dataStore;
        }

        protected override bool HasOption(char option)
        {
            return DataStore.ContainsKey(option.ToString());
        }

        protected override bool HasOption(string option)
        {
            return DataStore.ContainsKey(option);
        }

        protected override Type GetOptionType(char option) {
            return DataStore.GetDataType(option.ToString());
        }

        protected override Type GetOptionType(string option)
        {
            return DataStore.GetDataType(option);
        }
    }
}
