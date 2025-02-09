using CalqFramework.Cli.Serialization;
using CalqFramework.DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess {
    internal class CliDualKeyValueStore<TValue> : DualKeyValueStoreBase<string, TValue>, ICliStore<string, TValue, MemberInfo> {

        private ICliStore<string, TValue, MemberInfo> _primaryStore;
        private ICliStore<string, TValue, MemberInfo> _secondarStore;

        private BindingFlags BindingAttr { get; }
        private IClassMemberSerializer CliSerializer { get; }

        public override IKeyValueStore<string, TValue> PrimaryAccessor => _primaryStore;

        public override IKeyValueStore<string, TValue> SecondaryAccessor => _secondarStore;

        public CliDualKeyValueStore(ICliStore<string, TValue, MemberInfo> primaryStore, ICliStore<string, TValue, MemberInfo> secondaryStore, BindingFlags bindingAttr, IClassMemberSerializer cliSerializer) {
            _primaryStore = primaryStore;
            _secondarStore = secondaryStore;
            BindingAttr = bindingAttr;
            CliSerializer = cliSerializer;
        }

        public IDictionary<MemberInfo, IEnumerable<string>> GetKeysByAccessors() {
            return _primaryStore.GetKeysByAccessors().Concat(_secondarStore.GetKeysByAccessors()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}
