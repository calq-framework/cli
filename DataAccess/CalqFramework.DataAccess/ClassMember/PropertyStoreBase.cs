using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {
    public abstract class PropertyStoreBase<TKey> : ClassDataMemberStoreBase<TKey, object?> {
        public PropertyStoreBase(object obj, BindingFlags bindingAttr) : base(obj, bindingAttr) {
        }

        public override object? this[MemberInfo accessor] {
            get {
                return ((PropertyInfo)accessor).GetValue(ParentObject);
            }
            set {
                ((PropertyInfo)accessor).SetValue(ParentObject, value);
            }
        }

        public override IEnumerable<MemberInfo> Accessors => ParentType.GetProperties().Where(ContainsAccessor);

        public override Type GetDataType(MemberInfo accessor) {
            return ((PropertyInfo)accessor).PropertyType;
        }

        public override object? GetValueOrInitialize(MemberInfo accessor) {
            var value = ((PropertyInfo)accessor).GetValue(ParentObject) ??
                   Activator.CreateInstance(((PropertyInfo)accessor).PropertyType) ??
                   Activator.CreateInstance(Nullable.GetUnderlyingType(((PropertyInfo)accessor).PropertyType)!)!;
            ((PropertyInfo)accessor).SetValue(ParentObject, value);
            return value;
        }
    }
}