using System.Collections.Generic;
using CalqFramework.Cli.InterfaceComponents;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess.InterfaceComponents {

    internal class SubcommandExecutorWithOptions : DistinctDualKeyValueStoreBase<string, string?>, ISubcommandExecutorWithOptions {
        private readonly IOptionStore _optionStore;
        private readonly ISubcommandExecutor _subcommandExecutor;

        public SubcommandExecutorWithOptions(ISubcommandExecutor subcommandExecutor, IOptionStore optionStore) {
            _subcommandExecutor = subcommandExecutor;
            _optionStore = optionStore;
        }

        protected override IKeyValueStore<string, string?> PrimaryStore => _subcommandExecutor;

        protected override IKeyValueStore<string, string?> SecondaryStore => _optionStore;

        public void AddArgument(string? value) {
            _subcommandExecutor.AddArgument(value);
        }

        public IEnumerable<Option> GetOptions() {
            return _optionStore.GetOptions();
        }

        public IEnumerable<Parameter> GetParameters() {
            return _subcommandExecutor.GetParameters();
        }

        public Parameter? GetFirstUnassignedParameter() {
            return _subcommandExecutor.GetFirstUnassignedParameter();
        }

        public object? Invoke() {
            return _subcommandExecutor.Invoke();
        }
    }
}
