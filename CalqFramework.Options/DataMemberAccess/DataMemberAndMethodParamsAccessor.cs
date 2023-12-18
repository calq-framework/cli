using CalqFramework.Options.Attributes;
using CalqFramework.Serialization.DataMemberAccess;
using CalqFramework.Serialization.Text;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Options.DataMemberAccess {
    public class DataMemberAndMethodParamsAccessor {
        public object Obj { get; }
        public IDataMemberAccessor DataMemberAccessor { get; }
        public MethodParamsAccessor MethodParamsAccessor { get; }

        public DataMemberAndMethodParamsAccessor(object obj, IDataMemberAccessor dataMemberAccessor, MethodParamsAccessor methodParamsAccessor) {
            Obj = obj;
            DataMemberAccessor = dataMemberAccessor;
            MethodParamsAccessor = methodParamsAccessor;
        }

        public bool TryResolveDataMemberKey(string dataMemberKey, out string result) {
            result = default!;

            var paramSuccess = MethodParamsAccessor.TryResolveDataMemberKey(dataMemberKey, out var paramResult);
            var dataMemberResult = DataMemberAccessor.GetDataMember(Obj.GetType(), dataMemberKey);
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
            var dataMemberResult = DataMemberAccessor.GetDataMember(Obj.GetType(), dataMemberKey);
            var dataMemberSuccess = dataMemberResult != null;

            if (paramSuccess && dataMemberSuccess) {
                throw new Exception("collision");
            }

            if (paramSuccess) {
                result = paramResult!;
                return true;
            }

            if (dataMemberSuccess) {
                result = DataMemberAccessor.GetDataMemberType(Obj.GetType(), dataMemberKey);
                return true;
            }

            return false;
        }

        //object? GetDataValue(string dataMemberKey);

        public void SetDataValue(string dataMemberKey, object? value) {
            var paramSuccess = MethodParamsAccessor.TrySetDataValue(dataMemberKey, value);
            var dataMemberResult = DataMemberAccessor.GetDataMember(Obj.GetType(), dataMemberKey);
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
                var type = DataMemberAccessor.GetDataMemberType(Obj.GetType(), dataMemberKey);
                var isCollection = type.GetInterface(nameof(ICollection)) != null;
                if (isCollection == false) {
                    DataMemberAccessor.SetDataMemberValue(Obj, dataMemberKey, value);
                } else {
                    var collection = DataMemberAccessor.GetDataMemberValue(Obj, dataMemberKey);
                    if (collection == null) {
                        collection = Activator.CreateInstance(type);
                        DataMemberAccessor.SetDataMemberValue(Obj, dataMemberKey, collection);
                    }
                    CollectionMemberAccessor.AddChildValue((collection as ICollection)!, value);
                }
            }
        }
    }
}