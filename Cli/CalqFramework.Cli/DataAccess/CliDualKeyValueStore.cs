using CalqFramework.Cli.Serialization;
using CalqFramework.DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess {
    internal class CliDualKeyValueStore<TValue> : DualKeyValueStoreBase<string, TValue>, ICliKeyValueStore<string, TValue, MemberInfo> {

        private ICliKeyValueStore<string, TValue, MemberInfo> _primaryStore;
        private ICliKeyValueStore<string, TValue, MemberInfo> _secondarStore;

        protected override IKeyValueStore<string, TValue> PrimaryStore => _primaryStore;

        protected override IKeyValueStore<string, TValue> SecondaryStore => _secondarStore;

        public CliDualKeyValueStore(ICliKeyValueStore<string, TValue, MemberInfo> primaryStore, ICliKeyValueStore<string, TValue, MemberInfo> secondaryStore) {
            _primaryStore = primaryStore;
            _secondarStore = secondaryStore;
        }

        public IDictionary<MemberInfo, IEnumerable<string>> GetKeysByAccessors() {
            return _primaryStore.GetKeysByAccessors().Concat(_secondarStore.GetKeysByAccessors()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}
