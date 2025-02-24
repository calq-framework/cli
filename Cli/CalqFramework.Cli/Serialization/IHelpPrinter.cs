using System;
using System.Collections.Generic;
using CalqFramework.Cli.InterfaceComponents;

namespace CalqFramework.Cli.Serialization {
    public interface IHelpPrinter {
        void PrintHelp(Type rootType, Submodule rootSubmodule, IEnumerable<Submodule> submodules, IEnumerable<Subcommand> subcommands, IEnumerable<Option> options);
        void PrintHelp(Type rootType, IEnumerable<Submodule> submodules, IEnumerable<Subcommand> subcommands, IEnumerable<Option> options);
        void PrintSubcommandHelp(Type rootType, Subcommand subcommand, IEnumerable<Option> options);
    }
}
