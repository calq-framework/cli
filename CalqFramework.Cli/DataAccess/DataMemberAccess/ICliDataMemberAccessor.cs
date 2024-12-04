using CalqFramework.Cli.Serialization;
using System.Reflection;

namespace CalqFramework.Serialization.DataAccess {
    public interface ICliDataMemberStore : IKeyValueStore<string, object?, MemberInfo> {
        public ICliDataMemberSerializer CliSerializer { get; }
    }
}