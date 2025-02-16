using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.InterfaceComponents {
    public class Submodule {
        public IEnumerable<string> Keys { get; init; }
        public MemberInfo MemberInfo { get; init; }
    }
}
