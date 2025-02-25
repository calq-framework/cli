using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.InterfaceComponents {
    public class Submodule {
        public required IEnumerable<string> Keys { get; init; }
        public required MemberInfo MemberInfo { get; init; }
    }
}
