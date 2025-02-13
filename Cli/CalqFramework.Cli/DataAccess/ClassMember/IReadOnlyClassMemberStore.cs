using CalqFramework.DataAccess;
using System.Collections.Generic;

namespace CalqFramework.Cli.DataAccess.ClassMember {
    public interface IReadOnlyClassMemberStore<TKey, TValue, TAccessor> : IReadOnlyKeyValueStore<TKey, TValue> {
        IDictionary<TAccessor, IEnumerable<TKey>> GetKeysByAccessors();
    }
}