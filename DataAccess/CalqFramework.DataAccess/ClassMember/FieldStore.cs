using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {
    sealed public class FieldStore : FieldStoreBase<string, object?>, IKeyValueStore<string, object?, MemberInfo> {
        public FieldStore(object obj, BindingFlags bindingAttr) : base(obj, bindingAttr) {
        }

        public override bool ContainsAccessor(MemberInfo accessor) {
            return accessor is FieldInfo && accessor.DeclaringType == ParentType;
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out MemberInfo result) {
            result = ParentType.GetField(key, BindingAttr);
            return result != null;
        }

        protected override object? ConvertFromInternalValue(MemberInfo accessor, object? value) {
            return value;
        }

        protected override object? ConvertToInternalValue(MemberInfo accessor, object? value) {
            return value;
        }
    }
}