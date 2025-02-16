using System.Reflection;
using CalqFramework.DataAccess;
using CalqFramework.DataAccess.ClassMember;

namespace CalqFramework.DataAccess.ClassMember;
public sealed class ClassDataMemberStoreFactory : ClassDataMemberStoreFactoryBase {

    protected override IKeyValueStore<string, object?> CreateFieldStore(object obj) {
        return new FieldStore(obj, BindingAttr);
    }

    protected override IKeyValueStore<string, object?> CreatePropertyStore(object obj) {
        return new PropertyStore(obj, BindingAttr);
    }
}
