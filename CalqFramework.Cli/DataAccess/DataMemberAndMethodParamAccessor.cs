using CalqFramework.Serialization.DataAccess;
using CalqFramework.Serialization.DataAccess.DataMemberAccess;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess
{
    internal class DataMemberAndMethodParamAccessor : DualDataAccessor
    {
        private MethodParamAccessor MethodParamsAccessor { get; }

        public ParameterInfo[] Parameters { get => MethodParamsAccessor.Parameters; }

        public DataMemberAndMethodParamAccessor(IDataMemberAccessor dataMemberAccessor, MethodParamAccessor methodParamsAccessor) : base(dataMemberAccessor, methodParamsAccessor)
        {
            MethodParamsAccessor = methodParamsAccessor;
        }

        public object? Invoke()
        {
            return MethodParamsAccessor.Invoke();
        }

        public string GetKey(int index)
        {
            return MethodParamsAccessor.GetKey(index);
        }

        public Type GetType(int index)
        {
            return MethodParamsAccessor.GetType(index);
        }

        public void SetValue(int index, object? value)
        {
            MethodParamsAccessor.SetValue(index, value);
        }
    }
}