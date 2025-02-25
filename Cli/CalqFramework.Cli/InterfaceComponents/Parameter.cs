using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.InterfaceComponents {

    public class Parameter {
        public required bool HasDefaultValue { get; init; }
        public required IEnumerable<string> Keys { get; init; }
        public required ParameterInfo ParameterInfo { get; init; }
        public required Type Type { get; init; }
        public string? Value { get; init; }
    }
}
