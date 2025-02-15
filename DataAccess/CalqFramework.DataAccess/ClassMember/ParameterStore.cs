using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {
    public class ParameterStore : ParameterStoreBase<string, object?>, IKeyValueStore<string, object?, ParameterInfo> {
        public ParameterStore(MethodInfo method) : base(method) {
        }

        public override bool ContainsAccessor(ParameterInfo accessor) {
            return accessor.Member == ParentMethod;
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out ParameterInfo result) {
            result = Parameters.FirstOrDefault(x => x.Name== key);
            return result != null;
        }

        protected override object? ConvertFromInternalValue(object? value, ParameterInfo accessor) {
            return value;
        }

        protected override object? ConvertToInternalValue(object? value, ParameterInfo accessor) {
            return value;
        }
    }
}
