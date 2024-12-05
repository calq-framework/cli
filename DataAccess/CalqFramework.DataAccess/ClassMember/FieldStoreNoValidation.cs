using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {
    sealed public class FieldStoreNoValidation : FieldStoreBase<string> {
        public FieldStoreNoValidation(object obj, BindingFlags bindingAttr) : base(obj, bindingAttr) {
        }

        protected override MemberInfo? GetClassMember(string key) {
            return ParentType.GetField(key, BindingAttr);
        }

        public override bool ContainsAccessor(MemberInfo accessor) {
            return true;
        }
    }
}