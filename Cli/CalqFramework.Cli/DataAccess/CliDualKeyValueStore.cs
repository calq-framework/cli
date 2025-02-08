using CalqFramework.Cli.Serialization;
using CalqFramework.DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess {
    internal class CliDualKeyValueStore : DualKeyValueStoreBase<string, object?>, ICliStore<string, object?, MemberInfo> {

        private ICliStore<string, object?, MemberInfo> _primaryStore;
        private ICliStore<string, object?, MemberInfo> _secondarStore;

        private BindingFlags BindingAttr { get; }
        private IClassMemberSerializer CliSerializer { get; }

        public override IKeyValueStore<string, object?> PrimaryAccessor => _primaryStore;

        public override IKeyValueStore<string, object?> SecondaryAccessor => _secondarStore;

        public CliDualKeyValueStore(ICliStore<string, object?, MemberInfo> primaryStore, ICliStore<string, object?, MemberInfo> secondaryStore, BindingFlags bindingAttr, IClassMemberSerializer cliSerializer) {
            _primaryStore = primaryStore;
            _secondarStore = secondaryStore;
            BindingAttr = bindingAttr;
            CliSerializer = cliSerializer;
        }

        // TODO create dual base then in concrete define Primary and Secondary
        // also here define prmary and secondary, set private fields ans primary and secondary from cosntructor
        // then access getbykeys from primary and secondary here
        public IDictionary<MemberInfo, IEnumerable<string>> GetKeysByAccessors() {
            return _primaryStore.GetKeysByAccessors().Concat(_secondarStore.GetKeysByAccessors()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}
