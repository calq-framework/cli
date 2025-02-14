using CalqFramework.DataAccess;
using System.Collections.Generic;

namespace CalqFramework.Cli.DataAccess.ClassMember {
    public interface IMyFunctionExecutor<TKey, TValue, TAccessor> : IFunctionExecutor<TKey, TValue> {
        IDictionary<TAccessor, IEnumerable<TKey>> GetKeysByAccessors();
    }
}