using CalqFramework.Cli.DataAccess.InterfaceComponent;
using CalqFramework.DataAccess.ClassMember;
using System.Reflection;

namespace CalqFramework.Cli {
    public class CliDeserializerOptions {
        public ICliComponentFactory CliOptionsStoreFactory { get; init; }
        public bool SkipUnknown { get; init; } = false;

        public CliDeserializerOptions() {
            CliOptionsStoreFactory = new CliComponentFactory();
        }
    }
}
