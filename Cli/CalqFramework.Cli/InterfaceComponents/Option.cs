using System;

using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.InterfaceComponents {

    public class Option {
        public IEnumerable<string> Keys { get; init; }
        public MemberInfo MemberInfo { get; init; }
        public Type Type { get; init; }
        public string? Value { get; init; }
    }
}