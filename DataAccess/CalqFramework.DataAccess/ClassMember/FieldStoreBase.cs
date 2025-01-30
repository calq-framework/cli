using CalqFramework.DataAccess;
using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {
    public abstract class FieldStoreBase<TKey> : ClassDataMemberResolverBase<TKey, object?>, IKeyValueStore<TKey, object?, MemberInfo> {
        public IEnumerable<MemberInfo> Accessors => ParentType.GetFields();

        public virtual object? this[MemberInfo accessor] {
            get {
                return ((FieldInfo)accessor).GetValue(ParentObject);
            }
            set {
                ((FieldInfo)accessor).SetValue(ParentObject, value);
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

        public FieldStoreBase(object obj, BindingFlags bindingAttr) : base(obj, bindingAttr) {
        }

        public Type GetDataType(MemberInfo accessor) {
            return ((FieldInfo)accessor).FieldType;
        }

        public object GetValueOrInitialize(MemberInfo accessor) {
            var value = ((FieldInfo)accessor).GetValue(ParentObject) ??
                   Activator.CreateInstance(((FieldInfo)accessor).FieldType) ??
                   Activator.CreateInstance(Nullable.GetUnderlyingType(((FieldInfo)accessor).FieldType)!)!;
            ((FieldInfo)accessor).SetValue(ParentObject, value);
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

        public abstract bool ContainsAccessor(MemberInfo accessor);
    }
}