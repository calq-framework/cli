using CalqFramework.Serialization.DataAccess;
using CalqFramework.Serialization.DataAccess.DataMemberAccess;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess
{
    internal class DataMemberAndMethodParamAccessor : DualDataAccessor
    {
        public IDataMemberAccessor DataMemberAccessor { get; }
        public MethodParamAccessor MethodParamsAccessor { get; }

        public ParameterInfo[] Parameters { get => MethodParamsAccessor.Parameters; }

        public DataMemberAndMethodParamAccessor(IDataMemberAccessor dataMemberAccessor, MethodParamAccessor methodParamsAccessor) : base(dataMemberAccessor, methodParamsAccessor)
        {
            DataMemberAccessor = dataMemberAccessor;
            MethodParamsAccessor = methodParamsAccessor;
        }

        public object? Invoke()
        {
            return MethodParamsAccessor.Invoke(DataMemberAccessor.Obj);
        }
    }
}