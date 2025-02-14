using CalqFramework.Cli.InterfaceComponents;
using CalqFramework.DataAccess;
using System.Collections.Generic;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    internal class SubcommandExecutorWithOptions : DualKeyValueStoreBase<string, string?>, ISubcommandExecutorWithOptions {
        private ISubcommandExecutor _subcommandExeutor;
        private IOptionStore _optionStore;

        public SubcommandExecutorWithOptions(ISubcommandExecutor subcommandExecutor, IOptionStore optionStore) {
            _subcommandExeutor = subcommandExecutor;
            _optionStore = optionStore;
        }

        protected override IKeyValueStore<string, string?> PrimaryStore => _subcommandExeutor;

        protected override IKeyValueStore<string, string?> SecondaryStore => _optionStore;

        public void AddParameter(string? value) {
            _subcommandExeutor.AddParameter(value);
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