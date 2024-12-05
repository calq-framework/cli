using CalqFramework.DataAccess;
using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {
    public abstract class PropertyStoreBase<TKey> : ClassDataMemberResolverBase<TKey, object?>, IKeyValueStore<TKey, object?, MemberInfo> {
        public IEnumerable<MemberInfo> Accessors => ParentType.GetProperties();

        public virtual object? this[MemberInfo accessor] {
            get {
                return ((PropertyInfo)accessor).GetValue(ParentObject);
            }
            set {
                ((PropertyInfo)accessor).SetValue(ParentObject, value);
            }
        }

        public object? this[TKey key] {
            get {
                var accessor = GetAccessor(key);
                return this[accessor];
            }
            set {
                var accessor = GetAccessor(key);
                this[accessor] = value;
            }
        }

        public PropertyStoreBase(object obj, BindingFlags bindingAttr) : base(obj, bindingAttr) {
        }

        public Type GetDataType(MemberInfo accessor) {
            return ((PropertyInfo)accessor).PropertyType;
        }

        public object GetValueOrInitialize(MemberInfo accessor) {
            var value = ((PropertyInfo)accessor).GetValue(ParentObject) ??
                   Activator.CreateInstance(((PropertyInfo)accessor).PropertyType) ??
                   Activator.CreateInstance(Nullable.GetUnderlyingType(((PropertyInfo)accessor).PropertyType)!)!;
            ((PropertyInfo)accessor).SetValue(ParentObject, value);
            return value;
        }

        public object GetValueOrInitialize(TKey key) {
            var accessor = GetAccessor(key);
            return GetValueOrInitialize(accessor);
        }

        public Type GetDataType(TKey key) {
            var accessor = GetAccessor(key);
            return GetDataType(accessor);
        }

        public string AccessorToString(MemberInfo accessor) {
            return accessor.Name;
        }

        public abstract bool ContainsAccessor(MemberInfo accessor);
    }
}