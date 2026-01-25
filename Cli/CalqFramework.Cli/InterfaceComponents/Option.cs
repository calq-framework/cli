using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.InterfaceComponents {

    /// <summary>
    /// Represents a CLI option (field or property) with its metadata.
    /// </summary>
    public class Option {
        public required IEnumerable<string> Keys { get; init; }
        public required MemberInfo MemberInfo { get; init; }
        public required Type Type { get; init; }
        public string? Value { get; init; }
    }
}
