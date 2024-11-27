using CalqFramework.Cli.Serialization;
using System.Reflection;

namespace CalqFramework.Serialization.DataAccess {
    public interface ICliDataMemberAccessor : IDataAccessor<string, object?, MemberInfo> {
        public ICliDataMemberSerializer CliSerializer { get; }
    }
}