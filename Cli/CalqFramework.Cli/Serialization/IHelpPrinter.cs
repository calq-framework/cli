using System;
using System.Collections.Generic;
using CalqFramework.Cli.InterfaceComponents;

namespace CalqFramework.Cli.Serialization {
    public interface IHelpPrinter {
        object? PrintHelp(Type rootType, Submodule rootSubmodule, IEnumerable<Submodule> submodules, IEnumerable<Subcommand> subcommands, IEnumerable<Option> options);
        object? PrintHelp(Type rootType, IEnumerable<Submodule> submodules, IEnumerable<Subcommand> subcommands, IEnumerable<Option> options);
        object? PrintSubcommandHelp(Type rootType, Subcommand subcommand, IEnumerable<Option> options);
    }
}
