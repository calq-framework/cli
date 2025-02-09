﻿using CalqFramework.Cli.InterfaceComponents;
using CalqFramework.DataAccess;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    public interface ISubmoduleStore : IKeyValueStore<string, object?> {
        IEnumerable<Submodule> GetSubmodules();
    }
}