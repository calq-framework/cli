namespace CalqFramework.DataAccess.CollectionElementStores;

/// <summary>
///     Provides element store for IEnumerable using immutable append operations.
///     Uses Enumerable.Append to create new enumerables rather than mutating.
/// </summary>
public sealed class EnumerableElementStore(IEnumerable enumerable) : CollectionElementStoreBase<string, object?> {
    public IEnumerable<object?> TargetEnumerable { get; set; } = enumerable.Cast<object?>();

    public override object? this[string key] {
        get {
            int index = int.Parse(key);
            return TargetEnumerable.ElementAtOrDefault(index);
        }
        set => throw DataAccessErrors.OperationNotSupported("Indexed set");
    }

    public override bool ContainsKey(string key) {
        if (!int.TryParse(key, out int index)) {
            return false;
        }

        return index >= 0 && index < TargetEnumerable.Count();
    }

    public override Type GetValueType(string key) {
        Type? elementType = TargetEnumerable.GetType()
            .GetGenericArguments()
            .FirstOrDefault();
        return elementType ?? typeof(object);
    }

    public override object? GetValueOrInitialize(string key) {
        int index = int.Parse(key);
        object? element = TargetEnumerable.ElementAtOrDefault(index);
        if (element == null) {
            Type elementType = GetValueType(key);
            element = elementType.CreateInstance();
        }

        return element;
    }

    public override IEnumerable Append(object? value) {
        TargetEnumerable = TargetEnumerable.Append(value);
        return TargetEnumerable;
    }
}
