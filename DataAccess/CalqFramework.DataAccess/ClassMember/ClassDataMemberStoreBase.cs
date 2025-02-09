using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {
    public abstract class ClassDataMemberStoreBase<TKey, TValue, TAccessor, TInternalValue> : KeyValueStoreBase<TKey, TValue, TAccessor, TInternalValue> {

        public ClassDataMemberStoreBase(object obj, BindingFlags bindingAttr) {
            ParentObject = obj;
            BindingAttr = bindingAttr;
            ParentType = obj.GetType();
        }

        protected object ParentObject { get; }
        protected Type ParentType { get; }
        protected BindingFlags BindingAttr { get; }

        protected override MissingMemberException CreateMissingMemberException(TKey key) {
            return new MissingMemberException($"Missing {key} in {ParentType}.");
        }
    }
}
