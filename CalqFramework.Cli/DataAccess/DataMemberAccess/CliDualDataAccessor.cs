using CalqFramework.Cli.Serialization;
using CalqFramework.Serialization.DataAccess;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.DataMemberAccess {
    internal class CliDualKeyValueStore : DualDataAccessor<string, object?, MemberInfo>, ICliDataMemberStore {
        public ICliDataMemberSerializer CliSerializer { get; }

        public CliDualKeyValueStore(IKeyValueStore<string, object?, MemberInfo> primaryStore, IKeyValueStore<string, object?, MemberInfo> secondaryStore, ICliDataMemberSerializerFactory cliSerializerFactory) : base(primaryStore, secondaryStore) {
            CliSerializer = cliSerializerFactory.CreateCliSerializer(() => Accessors, (x) => GetDataType(x), (x) => this[x]);
        }
    }
}
