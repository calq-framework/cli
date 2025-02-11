using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {
    sealed public class PropertyStore : PropertyStoreBase<string, object?>, IKeyValueStore<string, object?, MemberInfo> {
        public PropertyStore(object obj, BindingFlags bindingAttr) : base(obj, bindingAttr) {
        }

        public override bool ContainsAccessor(MemberInfo accessor) {
            return accessor is PropertyInfo && accessor.DeclaringType == ParentType;
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out MemberInfo result) {
            result = ParentType.GetProperty(key, BindingAttr);
            return result != null;
        }

        protected override object? ConvertFromInternalValue(object? value, MemberInfo accessor) {
            return value;
        }

        protected override object? ConvertToInternalValue(object? value, MemberInfo accessor) {
            return value;
        }
    }
}