using CalqFramework.DataAccess.ClassMemberStores;

namespace CalqFramework.DataAccess;

/// <summary>
///     Factory for creating stores that access class members (fields and properties).
/// </summary>
public sealed class ClassMemberStoreFactory : ClassMemberStoreFactoryBase {
    protected override IKeyValueStore<string, object?> CreateFieldStore(object obj) =>
        new FieldStore(obj, BindingFlags);

    protected override IKeyValueStore<string, object?> CreatePropertyStore(object obj) =>
        new PropertyStore(obj, BindingFlags);
}
