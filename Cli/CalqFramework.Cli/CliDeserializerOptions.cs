using CalqFramework.Cli.DataAccess;
using CalqFramework.DataAccess.ClassMember;
using System.Reflection;

namespace CalqFramework.Cli {
    public class CliDeserializerOptions {
        public CliOptionsStoreFactory CliOptionsStoreFactory { get; init; }
        public bool SkipUnknown { get; init; } = false;

        public CliDeserializerOptions() {
            CliOptionsStoreFactory = new CliOptionsStoreFactory();
        }
    }
}
