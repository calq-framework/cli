using System;
using CalqFramework.Extensions.System;

namespace CalqFramework.Cli.DataAccess {

    internal class ReadOnlyPassThroughConverter<TValue> : ICompositeValueConverter<TValue> {

        public bool IsConvertible(Type type) {
            return true;
        }

        public TValue ConvertFromInternalValue(object? value, Type internalType) {
            return (TValue)value!;
        }

        public object? ConvertToInternalValue(TValue value, Type internalType, object? currentValue) => throw new NotSupportedException();

        public bool IsCollection(Type type) {
            return type.IsCollection();
        }

        public Type GetDataType(Type type) {
            return type.GetCollectionElementType();
        }
    }
}
