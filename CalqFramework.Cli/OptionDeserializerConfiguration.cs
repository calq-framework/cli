using System.Reflection;
using CalqFramework.Cli.DataAccess;
using CalqFramework.DataAccess;
using CalqFramework.DataAccess.ClassMemberStores;

namespace CalqFramework.Cli {
    /// <summary>
    /// Configuration options for OptionDeserializer behavior.
    /// </summary>
    public class OptionDeserializerConfiguration {
        public bool SkipUnknown { get; init; } = false;
    }
}
