using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {
    public abstract class FieldStoreBase<TKey> : ClassDataMemberStoreBase<TKey, object?> {
        public FieldStoreBase(object obj, BindingFlags bindingAttr) : base(obj, bindingAttr) {
        }

        public override object? this[MemberInfo accessor] {
            get {
                return ((FieldInfo)accessor).GetValue(ParentObject);
            }
            set {
                ((FieldInfo)accessor).SetValue(ParentObject, value);
            }
        }

        public override IEnumerable<MemberInfo> Accessors => ParentType.GetFields();

        public override Type GetDataType(MemberInfo accessor) {
            return ((FieldInfo)accessor).FieldType;
        }

        public override object? GetValueOrInitialize(MemberInfo accessor) {
            var value = ((FieldInfo)accessor).GetValue(ParentObject) ??
                   Activator.CreateInstance(((FieldInfo)accessor).FieldType) ??
                   Activator.CreateInstance(Nullable.GetUnderlyingType(((FieldInfo)accessor).FieldType)!)!;
            ((FieldInfo)accessor).SetValue(ParentObject, value);
            return value;
        }
    }
}