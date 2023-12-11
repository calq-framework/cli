using CalqFramework.Options.Attributes;
using CalqFramework.Serialization.DataMemberAccess;
using Microsoft.VisualBasic.FileIO;
using System;
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

        //public (ICollection<List<string>> globalOptions, ICollection<List<string>> coreCommands) GetInstanceOptions(Type type) {
        //    var membersByKeys = DataMemberAccessor.GetDataMembersByKeys(Obj.GetType());
        //    var keysByMembers = membersByKeys.GroupBy(x => x.Value, x => x.Key).ToDictionary(x => x.Key, x => x.OrderByDescending(e => e.Length).ToList());
        //    var globalOptions = keysByMembers.Where(x => { var type = DataMemberAccessor.GetDataMemberType(Obj.GetType(), x.Value[0]); return type.IsPrimitive || type == typeof(string); }); // TODO if parseable
        //    var coreCommands = keysByMembers.Where(x => { var type = DataMemberAccessor.GetDataMemberType(Obj.GetType(), x.Value[0]); return !type.IsPrimitive && type != typeof(string); }); // TODO if parseable
        //    var result = (globalOptions.Select(x => x.Value).ToList(), coreCommands.Select(x => x.Value).ToList());
        //    return result;
        //}

        //ICollection<string> GetDataMembersByKeys() { }

        //ICollection<string> GetAssociatedDataKeys(string dataMemberKey);

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

            if (dataMemberSuccess) {
                //result = dataMemberResult;
                DataMemberAccessor.SetDataMemberValue(Obj, dataMemberKey, value);
            }
        }

        public void SetDataValue(int dataMemberKey, object? value) {
            MethodParamsAccessor.SetDataValue(dataMemberKey, value);
        }
    }
}