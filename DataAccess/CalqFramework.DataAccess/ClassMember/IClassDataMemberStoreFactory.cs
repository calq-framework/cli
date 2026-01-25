using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {
    /// <summary>
    /// Factory for creating key-value stores that access class data members (fields and properties).
    /// </summary>
    public interface IClassDataMemberStoreFactory {
        /// <summary>
        /// Creates a key-value store for accessing data members of the specified object.
        /// </summary>
        IKeyValueStore<string, object?> CreateDataMemberStore(object obj);
    }
}
