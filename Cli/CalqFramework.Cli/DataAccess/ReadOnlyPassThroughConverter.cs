using System;
using CalqFramework.Extensions.System;

namespace CalqFramework.Cli.DataAccess {

    internal class ReadOnlyPassThroughConverter<TValue> : ICompositeValueConverter<TValue> {

        public bool CanConvert(Type targetType) {
            return true;
        }

        public TValue ConvertFrom(object? value, Type targetType) {
            return (TValue)value!;
        }

        public object? ConvertToOrUpdate(TValue value, Type targetType, object? currentValue) => throw new NotSupportedException();

        public bool IsCollection(Type type) {
            return type.IsCollection();
        }

        public Type GetDataType(Type type) {
            return type.GetCollectionElementType();
        }
    }
}
