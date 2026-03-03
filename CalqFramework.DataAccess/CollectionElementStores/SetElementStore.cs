using System.Collections;
using System.Linq;

namespace CalqFramework.DataAccess.CollectionElementStores;

/// <summary>
/// Provides key-value access to set elements.
/// For sets, the key and value are the same - the element itself.
/// </summary>
public sealed class SetElementStore : GenericCollectionElementStoreBase<string, object?> {

    public SetElementStore(IEnumerable set) : base(set) {
    }

    public override object? this[string key] {
        get {
            // For sets, we need to find and return the actual element
            // The key is the element value we're looking for
            foreach (var item in Collection) {
                if (Equals(item, key)) {
                    return item;
                }
            }
            throw DataAccessErrors.InvalidListKey(key);
        }
        set {
            // Sets don't support replacing elements
            throw DataAccessErrors.OperationNotSupported("Indexed set on Set - use Add/Remove instead");
        }
    }

    public override bool ContainsKey(string key) {
        // Check if the set contains this element
        return Contains(key);
    }

    public override object? GetValueOrInitialize(string key) {
        // For sets, if the element doesn't exist, add it
        if (!Contains(key)) {
            Add(key);
        }
        // Return the element from the set
        return this[key];
    }

    public override void Remove(string key) {
        if (!Remove(key)) {
            throw DataAccessErrors.InvalidListKey(key);
        }
    }
}
