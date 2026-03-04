using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.InterfaceComponents;

namespace CalqFramework.Cli.DataAccess.InterfaceComponentStores;

internal class SubcommandStore : ISubcommandStore {
    public SubcommandStore(ICliReadOnlyKeyValueStore<string, MethodInfo, MethodInfo> store) => Store = store;

    private ICliReadOnlyKeyValueStore<string, MethodInfo, MethodInfo> Store { get; }

    public MethodInfo? this[string key] => Store[key];

    public bool ContainsKey(string key) => Store.ContainsKey(key);

    public Type GetValueType(string key) => Store.GetValueType(key);

    public IEnumerable<Subcommand> GetSubcommands(
        Func<MethodInfo, object?, ISubcommandExecutor> createSubcommandExecutor) =>
        Store.GetAccessorKeysPairs().Select(pair => new Subcommand {
            ReturnType = GetValueType(pair.Keys[0]),
            Keys = pair.Keys,
            MethodInfo = pair.Accessor,
            Parameters = createSubcommandExecutor(pair.Accessor, null).GetParameters()
        });
}
