﻿using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {
    public abstract class PropertyStoreBase<TKey, TValue> : ClassDataMemberStoreBase<TKey, TValue, PropertyInfo, object?> {
        public PropertyStoreBase(object obj, BindingFlags bindingAttr) : base(obj, bindingAttr) {
        }

        public override object? this[PropertyInfo accessor] {
            get {
                return ((PropertyInfo)accessor).GetValue(ParentObject);
            }
            set {
                ((PropertyInfo)accessor).SetValue(ParentObject, value);
            }
        }

        public override IEnumerable<PropertyInfo> Accessors => ParentType.GetProperties().Where(ContainsAccessor);

        public override Type GetDataType(PropertyInfo accessor) {
            return ((PropertyInfo)accessor).PropertyType;
        }

        public override object? GetValueOrInitialize(PropertyInfo accessor) {
            var value = ((PropertyInfo)accessor).GetValue(ParentObject) ??
                   Activator.CreateInstance(((PropertyInfo)accessor).PropertyType) ??
                   Activator.CreateInstance(Nullable.GetUnderlyingType(((PropertyInfo)accessor).PropertyType)!)!;
            ((PropertyInfo)accessor).SetValue(ParentObject, value);
            return value;
        }
    }
}