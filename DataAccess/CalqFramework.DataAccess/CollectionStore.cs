using System.Collections;
using CalqFramework.DataAccess.Text;

namespace CalqFramework.DataAccess;

// TODO consider CollectionStoreFactory and accessor per collection type
public class CollectionStore : IKeyValueStore<string, object?> {
    protected object ParentCollection { get; }

    public object? this[string key] {
        get {
            return ParentCollection switch {
                Array array =>
                    array.GetValue(int.Parse(key)),
                IList list =>
                    list[int.Parse(key)],
                IDictionary dictionary =>
                    dictionary[ValueParser.ParseValue(key, dictionary.GetType().GetGenericArguments()[0])],
                _ => throw new Exception("unsupported collection")
            };

        }
        set {
            switch (ParentCollection) {
                case Array array:
                    array.SetValue(value, int.Parse(key));
                    break;
                case IList list:
                    list[int.Parse(key)] = value;
                    break;
                case IDictionary dictionary:
                    dictionary[ValueParser.ParseValue(key, dictionary.GetType().GetGenericArguments()[0])] = value;
                    break;
                default:
                    throw new Exception("unsupported collection");
            }
        }
    }

    public CollectionStore(ICollection collection) {
        ParentCollection = collection;
    }


    public Type GetDataType(string key) {
        return this[key]!.GetType();
    }

    public object? GetValueOrInitialize(string key) {
        switch (ParentCollection) {
            case Array array:
                var arrayElement = array.GetValue(int.Parse(key));
                if (arrayElement == null) {
                    arrayElement = Activator.CreateInstance(ParentCollection.GetType().GetElementType()!) ??
                        Activator.CreateInstance(Nullable.GetUnderlyingType(ParentCollection.GetType().GetGenericArguments()[0])!)!;
                    array.SetValue(arrayElement, int.Parse(key));
                }
                return arrayElement;
            case IList list:
                var listElement = list[int.Parse(key)];
                if (listElement == null) {
                    listElement = Activator.CreateInstance(ParentCollection.GetType().GetGenericArguments()[0]!) ??
                        Activator.CreateInstance(Nullable.GetUnderlyingType(ParentCollection.GetType().GetGenericArguments()[0])!)!;
                    list[int.Parse(key)] = listElement;
                }
                return listElement;
            case IDictionary dictionary:
                var dictionaryElement = dictionary[ValueParser.ParseValue(key, dictionary.GetType().GetGenericArguments()[0])];
                if (dictionaryElement == null) {
                    dictionaryElement = Activator.CreateInstance(ParentCollection.GetType().GetGenericArguments()[1]!) ??
                        Activator.CreateInstance(Nullable.GetUnderlyingType(ParentCollection.GetType().GetGenericArguments()[0])!)!;
                    dictionary[ValueParser.ParseValue(key, dictionary.GetType().GetGenericArguments()[0])] = dictionaryElement;
                }
                return dictionaryElement;
            default:
                throw new Exception("unsupported collection");
        }
    }

    // FIXME
    public bool ContainsKey(string key) {
        throw new NotImplementedException();
    }

    public void AddValue(object? value) {
        switch (ParentCollection) {
            case IList list:
                list.Add(value);
                break;
            default:
                throw new Exception("unsupported collection");
        }
    }
    public object AddValue() {
        switch (ParentCollection) {
            case IList list:
                var value = Activator.CreateInstance(ParentCollection.GetType().GetGenericArguments()[0]) ??
                   Activator.CreateInstance(Nullable.GetUnderlyingType(ParentCollection.GetType().GetGenericArguments()[0])!)!;
                list.Add(value);
                return value;
            default:
                throw new Exception("unsupported collection");
        }
    }

    public void RemoveValue(string key) {
        switch (ParentCollection) {
            case IList list:
                list.RemoveAt(int.Parse(key));
                break;
            case IDictionary dictionary:
                dictionary.Remove(key);
                break;
            default:
                throw new Exception("unsupported collection");
        }
    }
}
