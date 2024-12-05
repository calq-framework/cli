using CalqFramework.Cli.Serialization;
using CalqFramework.Serialization.DataAccess;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.ClassMember {
    internal class CliDualKeyValueStore : DualDataAccessor<string, object?, MemberInfo>, ICliKeyValueStore {
        public ICliClassDataMemberSerializer CliSerializer { get; }

        public CliDualKeyValueStore(IKeyValueStore<string, object?, MemberInfo> primaryStore, IKeyValueStore<string, object?, MemberInfo> secondaryStore, ICliClassDataMemberSerializerFactory cliSerializerFactory) : base(primaryStore, secondaryStore) {
            CliSerializer = cliSerializerFactory.CreateCliSerializer(() => Accessors, (x) => GetDataType(x), (x) => this[x]);
        }
    }
}
