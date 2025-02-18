using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {

    public abstract class PropertyStoreBase<TKey, TValue> : ClassDataMemberStoreBase<TKey, TValue, PropertyInfo, object?> {

        public PropertyStoreBase(object obj, BindingFlags bindingFlags) : base(obj, bindingFlags) {
        }

        public override IEnumerable<PropertyInfo> Accessors => ParentType.GetProperties(BindingFlags).Where(ContainsAccessor);

        public override object? this[PropertyInfo accessor] {
            get {
                return ((PropertyInfo)accessor).GetValue(ParentObject);
            }
            set {
                ((PropertyInfo)accessor).SetValue(ParentObject, value);
            }
        }

        public override Type GetDataType(PropertyInfo accessor) {
            return ((PropertyInfo)accessor).PropertyType;
        }

        public override object? GetValueOrInitialize(PropertyInfo accessor) {
            object value = ((PropertyInfo)accessor).GetValue(ParentObject) ??
                   Activator.CreateInstance(((PropertyInfo)accessor).PropertyType) ??
                   Activator.CreateInstance(Nullable.GetUnderlyingType(((PropertyInfo)accessor).PropertyType)!)!;
            ((PropertyInfo)accessor).SetValue(ParentObject, value);
            return value;
        }
    }
}
