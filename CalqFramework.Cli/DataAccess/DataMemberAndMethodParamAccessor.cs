using CalqFramework.Cli.DataAccess.DataMemberAccess;
using CalqFramework.Serialization.DataAccess;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess {
    internal class DataMemberAndMethodParamAccessor : DualDataAccessor<string,  object?> {
        public IDataAccessor<string, object?> DataMemberAccessor { get; }
        public CliMethodParamAccessor MethodParamsAccessor { get; }
        public object ParentObj { get; }

        //        public ParameterInfo[] Parameters { get => MethodParamsAccessor.Parameters; }

        public DataMemberAndMethodParamAccessor(IDataAccessor<string, object?, MemberInfo?> dataMemberAccessor, CliMethodParamAccessor methodParamsAccessor, object parentObj) : base(dataMemberAccessor, methodParamsAccessor)
        {
            DataMemberAccessor = dataMemberAccessor;
            MethodParamsAccessor = methodParamsAccessor;
            ParentObj = parentObj;
        }

        public object? Invoke()
        {
            return MethodParamsAccessor.Invoke(ParentObj);
        }
    }
}