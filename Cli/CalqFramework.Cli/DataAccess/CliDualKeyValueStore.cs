using CalqFramework.Cli.Serialization;
using CalqFramework.DataAccess;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess {
    internal class CliDualKeyValueStore : DualKeyValueStore<string, object?, MemberInfo>, ICliOptionsStore {
        public ICliClassDataMemberSerializer CliSerializer { get; }

        public CliDualKeyValueStore(IKeyValueStore<string, object?, MemberInfo> primaryStore, IKeyValueStore<string, object?, MemberInfo> secondaryStore, ICliClassDataMemberSerializerFactory cliSerializerFactory) : base(primaryStore, secondaryStore) {
            CliSerializer = cliSerializerFactory.CreateCliSerializer(() => Accessors, (x) => GetDataType(x), (x) => this[x]);
        }
    }
}
