﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess {

    internal class CliDualKeyValueStore<TValue> : DualKeyValueStoreBase<string, TValue>, ICliKeyValueStore<string, TValue, MemberInfo> {
        private readonly ICliKeyValueStore<string, TValue, MemberInfo> _primaryStore;
        private readonly ICliKeyValueStore<string, TValue, MemberInfo> _secondarStore;

        public CliDualKeyValueStore(ICliKeyValueStore<string, TValue, MemberInfo> primaryStore, ICliKeyValueStore<string, TValue, MemberInfo> secondaryStore) {
            _primaryStore = primaryStore;
            _secondarStore = secondaryStore;
        }

        protected override IKeyValueStore<string, TValue> PrimaryStore => _primaryStore;

        protected override IKeyValueStore<string, TValue> SecondaryStore => _secondarStore;

        public IDictionary<MemberInfo, IEnumerable<string>> GetKeysByAccessors() {
            return _primaryStore.GetKeysByAccessors().Concat(_secondarStore.GetKeysByAccessors()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}
