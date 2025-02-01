using System.Collections.Generic;
using System.Reflection;
using System;

namespace CalqFramework.Cli.DataAccess {
    public class Option {
        public IEnumerable<string> Keys { get; init; }
        public MemberInfo MemberInfo{ get; init; }
        public Type Type { get; init; }
        public string? Value { get; init; }
    }
}