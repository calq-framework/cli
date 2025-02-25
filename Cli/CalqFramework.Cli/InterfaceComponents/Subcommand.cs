using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.InterfaceComponents {

    public class Subcommand {
        public required IEnumerable<string> Keys { get; init; }
        public required MethodInfo MethodInfo { get; init; }
        public required IEnumerable<Parameter> Parameters { get; init; }
        public required Type ReturnType { get; init; }
    }
}
