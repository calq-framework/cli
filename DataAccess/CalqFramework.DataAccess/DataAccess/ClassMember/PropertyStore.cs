using System.Reflection;

namespace CalqFramework.Serialization.DataAccess.ClassMember {
    sealed public class PropertyStore : PropertyStoreBase<string> {
        public PropertyStore(object obj, BindingFlags bindingAttr) : base(obj, bindingAttr) {
        }

        protected override MemberInfo? GetClassMember(string key) {
            return ParentType.GetProperty(key, BindingAttr);
        }

        public override bool ContainsAccessor(MemberInfo accessor) {
            return accessor is PropertyInfo && accessor.DeclaringType == ParentType;
        }
    }
}