using CalqFramework.Serialization.DataAccess;
using CalqFramework.Serialization.DataAccess.DataMemberAccess;
using System;
using System.Collections;

namespace CalqFramework.Options.DataMemberAccess {
    public class DataMemberAndMethodParamsAccessor {
        public DataMemberAccessorBase DataMemberAccessor { get; }
        public MethodParamsAccessor MethodParamsAccessor { get; }

        public DataMemberAndMethodParamsAccessor(DataMemberAccessorBase dataMemberAccessor, MethodParamsAccessor methodParamsAccessor) {
            DataMemberAccessor = dataMemberAccessor;
            MethodParamsAccessor = methodParamsAccessor;
        }

        public bool TryResolveDataMemberKey(string dataMemberKey, out string result) {
            result = default!;

            var paramSuccess = MethodParamsAccessor.TryResolveDataMemberKey(dataMemberKey, out var paramResult);
            var dataMemberResult = DataMemberAccessor.GetDataMember(dataMemberKey);
            var dataMemberSuccess = dataMemberResult != null;

            if (paramSuccess && dataMemberSuccess) {
                throw new Exception("collision");
            }

            if (paramSuccess) {
                result = paramResult!;
                return true;
            }

            if (dataMemberSuccess) {
                result = dataMemberResult!.Name;
                return true;
            }

            return false;
        }

        public bool TryGetDataType(string dataMemberKey, out Type result) {
            result = default!;

            var paramSuccess = MethodParamsAccessor.TryGetDataType(dataMemberKey, out var paramResult);
            var dataMemberResult = DataMemberAccessor.GetDataMember(dataMemberKey);
            var dataMemberSuccess = dataMemberResult != null;

            if (paramSuccess && dataMemberSuccess) {
                throw new Exception("collision");
            }

            if (paramSuccess) {
                result = paramResult!;
                return true;
            }

            if (dataMemberSuccess) {
                result = DataMemberAccessor.GetType(dataMemberKey);
                return true;
            }

            return false;
        }

        //object? GetDataValue(string dataMemberKey);

        public void SetDataValue(string dataMemberKey, object? value) {
            var paramSuccess = MethodParamsAccessor.TrySetDataValue(dataMemberKey, value);
            var dataMemberResult = DataMemberAccessor.GetDataMember(dataMemberKey);
            var dataMemberSuccess = dataMemberResult != null;

            if (paramSuccess && dataMemberSuccess) {
                throw new Exception("collision");
            }

            if (paramSuccess) {
                // result = paramResult;
            }

            // TODO unify collection handling logic with MethodParamsAccessor
            if (dataMemberSuccess) {
                //result = dataMemberResult;
                var type = DataMemberAccessor.GetType(dataMemberKey);
                var isCollection = type.GetInterface(nameof(ICollection)) != null;
                if (isCollection == false) {
                    DataMemberAccessor.SetValue(dataMemberKey, value);
                } else {
                    var collection = DataMemberAccessor.GetValue(dataMemberKey);
                    if (collection == null) {
                        collection = Activator.CreateInstance(type);
                        DataMemberAccessor.SetValue(dataMemberKey, collection);
                    }
                    CollectionAccessor.AddValue((collection as ICollection)!, value);
                }
            }
        }
    }
}