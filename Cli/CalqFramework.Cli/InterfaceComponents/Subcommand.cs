using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.InterfaceComponents {

    public class Subcommand {
        public IEnumerable<string> Keys { get; init; }
        public MethodInfo MethodInfo { get; init; }
        public IEnumerable<Parameter> Parameters { get; init; }
        public Type ReturnType { get; init; }
    }
}
