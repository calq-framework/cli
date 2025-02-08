using System.Collections.Generic;
using System.Reflection;
using System;

namespace CalqFramework.Cli.InterfaceComponents {
    public class Submodule {
        public IEnumerable<string> Keys { get; init; }
        public MemberInfo MemberInfo { get; init; }
    }
}