using CalqFramework.Cli.InterfaceComponents;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess.InterfaceComponentStores;

internal sealed class SubcommandExecutorWithOptions(ISubcommandExecutor subcommandExecutor, IOptionStore optionStore) : DistinctDualKeyValueStoreBase<string, string?>, ISubcommandExecutorWithOptions {
    private readonly IOptionStore _optionStore = optionStore;
    private readonly ISubcommandExecutor _subcommandExecutor = subcommandExecutor;

    protected override IKeyValueStore<string, string?> PrimaryStore => _subcommandExecutor;

    protected override IKeyValueStore<string, string?> SecondaryStore => _optionStore;

    public void AddArgument(string? value) => _subcommandExecutor.AddArgument(value);

    public IEnumerable<Option> GetOptions() => _optionStore.GetOptions();

    public IEnumerable<Parameter> GetParameters() => _subcommandExecutor.GetParameters();

    public Parameter? GetFirstUnassignedParameter() => _subcommandExecutor.GetFirstUnassignedParameter();

    public object? Invoke() => _subcommandExecutor.Invoke();
}
