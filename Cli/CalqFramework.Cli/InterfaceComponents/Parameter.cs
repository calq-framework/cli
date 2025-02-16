using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.InterfaceComponents {

    public class Parameter {
        public bool HasDefaultValue { get; init; }
        public IEnumerable<string> Keys { get; init; }
        public ParameterInfo ParameterInfo { get; init; }
        public Type Type { get; init; }
        public string? Value { get; init; }
    }
}