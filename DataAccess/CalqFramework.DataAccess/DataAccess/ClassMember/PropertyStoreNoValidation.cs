using System.Reflection;

namespace CalqFramework.Serialization.DataAccess.ClassMember {
    sealed public class PropertyStoreNoValidation : PropertyStoreBase<string> {
        public PropertyStoreNoValidation(object obj, BindingFlags bindingAttr) : base(obj, bindingAttr) {
        }

        protected override MemberInfo? GetClassMember(string key) {
            return ParentType.GetProperty(key, BindingAttr);
        }

        public override bool ContainsAccessor(MemberInfo accessor) {
            return true;
        }
    }
}