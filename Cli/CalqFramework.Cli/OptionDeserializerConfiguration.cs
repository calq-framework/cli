using System.Reflection;
using CalqFramework.Cli.DataAccess.InterfaceComponent;
using CalqFramework.DataAccess;
using CalqFramework.DataAccess.ClassMember;

namespace CalqFramework.Cli {
    /// <summary>
    /// Configuration options for OptionDeserializer behavior.
    /// </summary>
    public class OptionDeserializerConfiguration {
        public bool SkipUnknown { get; init; } = false;
    }
}
