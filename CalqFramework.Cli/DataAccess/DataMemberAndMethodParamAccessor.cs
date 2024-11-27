using CalqFramework.Serialization.DataAccess;

namespace CalqFramework.Cli.DataAccess {
    internal class DataMemberAndMethodParamAccessor : DualDataAccessor<string,  object?> {
        public ICliDataMemberAccessor DataMemberAccessor { get; }
        public CliMethodParamAccessor MethodParamsAccessor { get; }
        public object ParentObj { get; }

        //        public ParameterInfo[] Parameters { get => MethodParamsAccessor.Parameters; }

        public DataMemberAndMethodParamAccessor(ICliDataMemberAccessor dataMemberAccessor, CliMethodParamAccessor methodParamsAccessor, object parentObj) : base(dataMemberAccessor, methodParamsAccessor)
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