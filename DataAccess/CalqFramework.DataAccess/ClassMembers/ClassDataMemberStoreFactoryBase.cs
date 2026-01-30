using System.Reflection;

namespace CalqFramework.DataAccess.ClassMembers;

public abstract class ClassDataMemberStoreFactoryBase : IClassDataMemberStoreFactory {

    public const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

    public bool AccessFields { get; init; } = false;
    public bool AccessProperties { get; init; } = true;

    public BindingFlags BindingFlags { get; init; } = DefaultLookup;

    public virtual IKeyValueStore<string, object?> CreateDataMemberStore(object obj) {
        if (AccessFields && AccessProperties) {
            return CreateFieldAndPropertyStore(obj);
        } else if (AccessFields) {
            return CreateFieldStore(obj);
        } else if (AccessProperties) {
            return CreatePropertyStore(obj);
        } else {
            throw DataAccessErrors.NoAccessConfigured();
        }
    }

    protected virtual IKeyValueStore<string, object?> CreateFieldAndPropertyStore(object obj) {
        return new DualKeyValueStore<string, object?>(CreateFieldStore(obj), CreatePropertyStore(obj));
    }
    protected abstract IKeyValueStore<string, object?> CreateFieldStore(object obj);
    protected abstract IKeyValueStore<string, object?> CreatePropertyStore(object obj);
}
