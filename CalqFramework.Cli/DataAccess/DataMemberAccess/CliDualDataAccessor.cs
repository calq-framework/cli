using CalqFramework.Cli.Serialization;
using CalqFramework.Serialization.DataAccess;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.DataMemberAccess {
    internal class CliDualDataAccessor : DualDataAccessor<string, object?, MemberInfo>, ICliDataMemberAccessor {
        public ICliDataMemberSerializer CliSerializer { get; }

        public CliDualDataAccessor(IDataAccessor<string, object?, MemberInfo> primaryAccessor, IDataAccessor<string, object?, MemberInfo> secondaryAccessor, ICliDataMemberSerializerFactory cliSerializerFactory) : base(primaryAccessor, secondaryAccessor) {
            CliSerializer = cliSerializerFactory.CreateCliSerializer(() => DataMediators, (x) => GetDataType(x), (x) => this[x]);
        }
    }
}
