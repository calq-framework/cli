using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {
    public class MethodParamStore : MethodParamStoreBase<string, object?>, IKeyValueStore<string, object?, ParameterInfo> {
        public MethodParamStore(MethodInfo method) : base(method) {
        }

        public override bool ContainsAccessor(ParameterInfo accessor) {
            return accessor.Member == Method;
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out ParameterInfo result) {
            result = Parameters.FirstOrDefault(x => x.Name== key);
            return result != null;
        }

        protected override object? ConvertFromInternalValue(ParameterInfo accessor, object? value) {
            return value;
        }

        protected override object? ConvertToInternalValue(ParameterInfo accessor, object? value) {
            return value;
        }
    }
}
