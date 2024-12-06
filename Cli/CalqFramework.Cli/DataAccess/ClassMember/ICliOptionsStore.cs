using CalqFramework.Cli.Serialization;
using CalqFramework.DataAccess;
using System.Reflection;

namespace CalqFramework.DataAccess {
    public interface ICliOptionsStore : IKeyValueStore<string, object?, MemberInfo> {
        public ICliClassDataMemberSerializer CliSerializer { get; }
    }
}