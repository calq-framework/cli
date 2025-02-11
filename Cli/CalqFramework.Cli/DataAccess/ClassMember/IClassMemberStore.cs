using CalqFramework.DataAccess;
using System.Collections.Generic;

namespace CalqFramework.Cli.DataAccess.ClassMember {
    public interface IClassMemberStore<TKey, TValue, TAccessor> : IKeyValueStore<TKey, TValue> {
        IDictionary<TAccessor, IEnumerable<TKey>> GetKeysByAccessors();
    }
}