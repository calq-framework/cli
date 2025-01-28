using CalqFramework.DataAccess;
using CalqFramework.DataAccess.ClassMember;
using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember;
sealed public class ClassDataMemberStoreFactory : ClassDataMemberStoreFactoryBase<string, object?> {

    protected override IKeyValueStore<string, object?, MemberInfo> CreateFieldStore(object obj) {
        return AccessProperties == true ? new FieldStore(obj, BindingAttr) : new FieldStoreNoValidation(obj, BindingAttr);
    }

    protected override IKeyValueStore<string, object?, MemberInfo> CreatePropertyStore(object obj) {
        return AccessFields == true ? new PropertyStore(obj, BindingAttr) : new PropertyStoreNoValidation(obj, BindingAttr);
    }
}
