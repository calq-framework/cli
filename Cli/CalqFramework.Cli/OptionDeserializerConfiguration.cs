using System.Reflection;
using CalqFramework.Cli.DataAccess.InterfaceComponents;
using CalqFramework.DataAccess;
using CalqFramework.DataAccess.ClassMembers;

namespace CalqFramework.Cli {
    /// <summary>
    /// Configuration options for OptionDeserializer behavior.
    /// </summary>
    public class OptionDeserializerConfiguration {
        public bool SkipUnknown { get; init; } = false;
    }
}
