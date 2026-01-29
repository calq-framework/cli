using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.InterfaceComponents {

    /// <summary>
    /// Represents a CLI subcommand (method) with its metadata.
    /// </summary>
    public class Subcommand {
        public required IReadOnlyList<string> Keys { get; init; }
        public required MethodInfo MethodInfo { get; init; }
        public required IEnumerable<Parameter> Parameters { get; init; }
        public required Type ReturnType { get; init; }
    }
}
