using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess {
    public class Method {
        public IEnumerable<string> Keys { get; init; }
        public MethodInfo Methodinfo { get; init; }
        public Type ReturnType { get; init; }
        public IEnumerable<PositionalPram> PositionalParameters { get; init; }
    }
}