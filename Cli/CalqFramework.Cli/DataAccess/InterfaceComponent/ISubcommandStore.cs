using CalqFramework.Cli.InterfaceComponents;
using CalqFramework.DataAccess;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {

    public interface ISubcommandStore : IReadOnlyKeyValueStore<string, MethodInfo?> {

        IEnumerable<Subcommand> GetSubcommands(Func<MethodInfo, object?, ISubcommandExecutor> createSubcommandExecutor);
    }
}