using CalqFramework.DataAccess;
using CalqFramework.DataAccess.ClassMember;
using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember;
sealed public class ClassDataMemberStoreFactory : ClassDataMemberStoreFactoryBase {

    protected override IKeyValueStore<string, object?> CreateFieldStore(object obj) {
        return new FieldStore(obj, BindingAttr);
    }

    protected override IKeyValueStore<string, object?> CreatePropertyStore(object obj) {
        return new PropertyStore(obj, BindingAttr);
    }
}
