using CalqFramework.Cli.Serialization;
using System.Reflection;

namespace CalqFramework.Serialization.DataAccess {
    public interface ICliKeyValueStore : IKeyValueStore<string, object?, MemberInfo> {
        public ICliClassDataMemberSerializer CliSerializer { get; }
    }
}