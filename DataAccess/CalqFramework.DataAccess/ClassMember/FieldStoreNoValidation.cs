using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {
    sealed public class FieldStoreNoValidation : FieldStoreBase<string> {
        public FieldStoreNoValidation(object obj, BindingFlags bindingAttr) : base(obj, bindingAttr) {
        }

        public override bool ContainsAccessor(MemberInfo accessor) {
            return true;
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out MemberInfo result) {
            result = ParentType.GetField(key, BindingAttr);
            return result != null;
        }
    }
}