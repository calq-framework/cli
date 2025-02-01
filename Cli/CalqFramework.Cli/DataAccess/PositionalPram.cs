using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess {
    public class PositionalPram {
        public IEnumerable<string> Keys { get; init; }
        public ParameterInfo ParamInfo { get; init; }
        public Type Type { get; init; }
        public string? Value { get; init; }
        public bool HasDefaultValue { get; init; }
    }
}