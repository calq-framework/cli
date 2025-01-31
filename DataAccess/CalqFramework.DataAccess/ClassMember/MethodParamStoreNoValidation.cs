using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {
    public class MethodParamStoreNoValidation : MethodParamStoreBase<string> {
        public MethodParamStoreNoValidation(MethodInfo method) : base(method) {
        }

        public override bool ContainsAccessor(ParameterInfo accessor) {
            return true;
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out ParameterInfo result) {
            result = Parameters.FirstOrDefault(x => x.Name== key);
            return result != null;
        }
    }
}
