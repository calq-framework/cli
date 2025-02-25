using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.InterfaceComponents {

    public class Option {
        public required IEnumerable<string> Keys { get; init; }
        public required MemberInfo MemberInfo { get; init; }
        public required Type Type { get; init; }
        public string? Value { get; init; }
    }
}
