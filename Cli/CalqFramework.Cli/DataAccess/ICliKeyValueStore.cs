﻿using System.Collections.Generic;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess {

    public interface ICliKeyValueStore<TKey, TValue, TAccessor> : IKeyValueStore<TKey, TValue> {

        IDictionary<TAccessor, IEnumerable<TKey>> GetKeysByAccessors();
    }
}
