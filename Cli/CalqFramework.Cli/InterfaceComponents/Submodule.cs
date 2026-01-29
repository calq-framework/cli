using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.InterfaceComponents {
    /// <summary>
    /// Represents a CLI submodule (nested object) with its metadata.
    /// </summary>
    public class Submodule {
        public required IReadOnlyList<string> Keys { get; init; }
        public required MemberInfo MemberInfo { get; init; }
    }
}
