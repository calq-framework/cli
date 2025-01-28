using CalqFramework.Cli.Serialization;
using CalqFramework.DataAccess;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess {
    internal class CliDualKeyValueStore : DualKeyValueStore<string, object?, MemberInfo>, ICliOptionsStore {
        private BindingFlags BindingAttr { get; }
        private ICliClassDataMemberSerializer CliSerializer { get; }

        public CliDualKeyValueStore(IKeyValueStore<string, object?, MemberInfo> primaryStore, IKeyValueStore<string, object?, MemberInfo> secondaryStore, BindingFlags bindingAttr, ICliClassDataMemberSerializer cliSerializer) : base(primaryStore, secondaryStore) {
            BindingAttr = bindingAttr;
            CliSerializer = cliSerializer;
        }

        public string GetCommandsString() {
            return CliSerializer.GetCommandsString(Accessors, (x) => GetDataType(x), (x) => this[x], BindingAttr);
        }

        public string GetOptionsString() {
            return CliSerializer.GetOptionsString(Accessors, (x) => GetDataType(x), (x) => this[x], BindingAttr);
        }
    }
}
