using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {

    public abstract class ClassDataMemberStoreBase<TKey, TValue, TAccessor, TInternalValue> : KeyValueStoreBase<TKey, TValue, TAccessor, TInternalValue> {

        protected ClassDataMemberStoreBase(object obj, BindingFlags bindingFlags) {
            ParentObject = obj;
            BindingFlags = bindingFlags;
            ParentType = obj.GetType();
        }

        protected BindingFlags BindingFlags { get; }
        protected object ParentObject { get; }
        protected Type ParentType { get; }

        protected override MissingMemberException CreateMissingMemberException(TKey key) {
            return new MissingMemberException($"Missing {key} in {ParentType}.");
        }
    }
}
