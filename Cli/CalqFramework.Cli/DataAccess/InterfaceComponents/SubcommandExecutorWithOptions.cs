using System.Collections.Generic;
using CalqFramework.Cli.InterfaceComponents;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess.InterfaceComponents {

    internal class SubcommandExecutorWithOptions : DistinctDualKeyValueStoreBase<string, string?>, ISubcommandExecutorWithOptions {
        private readonly IOptionStore _optionStore;
        private readonly ISubcommandExecutor _subcommandExeutor;

        public SubcommandExecutorWithOptions(ISubcommandExecutor subcommandExecutor, IOptionStore optionStore) {
            _subcommandExeutor = subcommandExecutor;
            _optionStore = optionStore;
        }

        protected override IKeyValueStore<string, string?> PrimaryStore => _subcommandExeutor;

        protected override IKeyValueStore<string, string?> SecondaryStore => _optionStore;

        public void AddArgument(string? value) {
            _subcommandExeutor.AddArgument(value);
        }

        public IEnumerable<Option> GetOptions() {
            return _optionStore.GetOptions();
        }

        public IEnumerable<Parameter> GetParameters() {
            return _subcommandExeutor.GetParameters();
        }

        public object? Invoke() {
            return _subcommandExeutor.Invoke();
        }
    }
}
