using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {

    public class MethodExecutor : MethodExecutorBase<string, object?>, IKeyValueStore<string, object?, ParameterInfo> {

        public MethodExecutor(MethodInfo method, object obj) : base(method, obj) {
        }

        private int ParameterValuesSize { get; set; }

        public override void AddParameter(object? value) {
            ParameterValues[ParameterValuesSize++] = value;
        }

        public override bool ContainsAccessor(ParameterInfo accessor) {
            return accessor.Member == ParentMethod;
        }

        public override object? Invoke() {
            return ParentMethod.Invoke(ParentObject, ParameterValues);
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out ParameterInfo result) {
            result = ParameterInfos.FirstOrDefault(x => x.Name == key);
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
