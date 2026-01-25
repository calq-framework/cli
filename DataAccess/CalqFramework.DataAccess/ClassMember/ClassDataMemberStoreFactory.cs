using System.Reflection;
using CalqFramework.DataAccess;
using CalqFramework.DataAccess.ClassMember;

namespace CalqFramework.DataAccess.ClassMember;
/// <summary>
/// Factory for creating stores that access class data members (fields and properties).
/// </summary>
public sealed class ClassDataMemberStoreFactory : ClassDataMemberStoreFactoryBase {

    protected override IKeyValueStore<string, object?> CreateFieldStore(object obj) {
        return new FieldStore(obj, BindingFlags);
    }

    protected override IKeyValueStore<string, object?> CreatePropertyStore(object obj) {
        return new PropertyStore(obj, BindingFlags);
    }
}
