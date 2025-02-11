using CalqFramework.Cli.Serialization;
using CalqFramework.DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.ClassMember {
    internal class DualClassMemberStore<TValue> : DualKeyValueStoreBase<string, TValue>, IClassMemberStore<string, TValue, MemberInfo> {

        private IClassMemberStore<string, TValue, MemberInfo> _primaryStore;
        private IClassMemberStore<string, TValue, MemberInfo> _secondarStore;

        public override IKeyValueStore<string, TValue> PrimaryStore => _primaryStore;

        public override IKeyValueStore<string, TValue> SecondaryStore => _secondarStore;

        public DualClassMemberStore(IClassMemberStore<string, TValue, MemberInfo> primaryStore, IClassMemberStore<string, TValue, MemberInfo> secondaryStore) {
            _primaryStore = primaryStore;
            _secondarStore = secondaryStore;
        }

        public IDictionary<MemberInfo, IEnumerable<string>> GetKeysByAccessors() {
            return _primaryStore.GetKeysByAccessors().Concat(_secondarStore.GetKeysByAccessors()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}
