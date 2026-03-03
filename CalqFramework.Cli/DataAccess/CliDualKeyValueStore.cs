using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess {

    internal class CliDualKeyValueStore<TValue> : DualKeyValueStoreBase<string, TValue>, ICliKeyValueStore<string, TValue, MemberInfo> {
        private readonly ICliKeyValueStore<string, TValue, MemberInfo> _primaryStore;
        private readonly ICliKeyValueStore<string, TValue, MemberInfo> _secondaryStore;

        public CliDualKeyValueStore(ICliKeyValueStore<string, TValue, MemberInfo> primaryStore, ICliKeyValueStore<string, TValue, MemberInfo> secondaryStore) {
            _primaryStore = primaryStore;
            _secondaryStore = secondaryStore;
        }

        protected override IKeyValueStore<string, TValue> PrimaryStore => _primaryStore;

        protected override IKeyValueStore<string, TValue> SecondaryStore => _secondaryStore;

        public IEnumerable<AccessorKeysPair<MemberInfo>> GetAccessorKeysPairs() {
            return _primaryStore.GetAccessorKeysPairs()
                .Concat(_secondaryStore.GetAccessorKeysPairs());
        }

        public bool IsMultiValue(string key) {
            if (_primaryStore.ContainsKey(key)) {
                return _primaryStore.IsMultiValue(key);
            } else {
                return _secondaryStore.IsMultiValue(key);
            }
        }
    }
}
