using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {
    public class MethodParamStore : MethodParamStoreBase<string> {
        public MethodParamStore(MethodInfo method) : base(method) {
        }

        public override bool ContainsAccessor(ParameterInfo accessor) {
            return accessor.Member == Method;
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out ParameterInfo result) {
            result = Parameters.FirstOrDefault(x => x.Name== key);
            return result != null;
        }
    }
}
