using System.Collections;

namespace CalqFramework.DataAccess;

/// <summary>
/// Factory for creating collection stores based on collection type.
/// </summary>
public interface ICollectionStoreFactory {
    /// <summary>
    /// Creates an appropriate collection store for the given collection.
    /// </summary>
    CollectionStoreBase CreateStore(ICollection collection);
}
