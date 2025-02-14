using CalqFramework.Cli.InterfaceComponents;
using CalqFramework.DataAccess;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    public interface ISubcommandExecutor : IFunctionExecutor<string, string?> {
        IEnumerable<Parameter> GetParameters();
    }
}