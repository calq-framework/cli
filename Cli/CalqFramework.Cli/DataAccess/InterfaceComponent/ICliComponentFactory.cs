using System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    public interface ICliComponentFactory {
        bool AccessFields { get; init; }
        bool AccessProperties { get; init; }
        BindingFlags BindingAttr { get; init; }

        ISubmoduleStore<string, object?, MemberInfo> CreateSubmoduleStore(object obj);
        IOptionStore<string, object?, MemberInfo> CreateOptionStore(object obj);
    }
}