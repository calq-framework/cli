using System;
using System.Collections;
using System.Reflection;
using CalqFramework.Extensions.System;

namespace CalqFramework.DataAccess.CollectionElementStores;

/// <summary>
/// Base class for collection element stores that use reflection to access collection methods.
/// Suitable for collections that implement Add, Remove, Contains methods and Count property.
/// </summary>
public abstract class GenericCollectionElementStoreBase<TKey, TValue> : CollectionElementStoreBase<TKey, TValue> {

    private readonly MethodInfo? _addMethod;
    private readonly MethodInfo? _removeMethod;
    private readonly MethodInfo? _containsMethod;
    private readonly PropertyInfo? _countProperty;

    protected GenericCollectionElementStoreBase(IEnumerable targetCollection) {
        Collection = targetCollection;
        var collectionType = targetCollection.GetType();
        
        // Cache reflection lookups for performance
        _addMethod = collectionType.GetMethod("Add");
        _removeMethod = collectionType.GetMethod("Remove");
        _containsMethod = collectionType.GetMethod("Contains");
        _countProperty = collectionType.GetProperty("Count");
    }

    protected IEnumerable Collection { get; }
    protected int Count => (int?)_countProperty?.GetValue(Collection) ?? 0;

    public override Type GetValueType(TKey key) {
        var elementType = Collection.GetType().GetGenericArguments()[0];
        return elementType;
    }

    public override void Add(TValue value) {
        _addMethod?.Invoke(Collection, new object?[] { value });
    }

    public override IEnumerable Append(TValue value) {
        Add(value);
        return Collection;
    }

    public override TValue AddNew() {
        var elementType = GetValueType(default!);
        var newElement = (TValue)elementType.CreateInstance();
        Add(newElement);
        return newElement;
    }

    protected bool Contains(object? item) {
        if (_containsMethod != null && item != null) {
            var result = _containsMethod.Invoke(Collection, new[] { item });
            return result is bool b && b;
        }
        return false;
    }

    protected bool Remove(object? item) {
        if (_removeMethod != null && item != null) {
            var result = _removeMethod.Invoke(Collection, new[] { item });
            return result is bool b && b;
        }
        return false;
    }
}
