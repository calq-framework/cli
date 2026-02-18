namespace CalqFramework.DataAccess;

/// <summary>
/// Factory for creating key-value stores that access class members (fields and properties).
/// </summary>
public interface IClassMemberStoreFactory {
    /// <summary>
    /// Creates a key-value store for accessing class members of the specified object.
    /// </summary>
    IKeyValueStore<string, object?> CreateClassMemberStore(object obj);
}
