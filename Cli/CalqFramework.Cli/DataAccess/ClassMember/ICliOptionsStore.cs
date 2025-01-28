using System.Reflection;

namespace CalqFramework.DataAccess {
    public interface ICliOptionsStore : IKeyValueStore<string, object?, MemberInfo> {
        string GetCommandsString();
        string GetOptionsString();
    }
}