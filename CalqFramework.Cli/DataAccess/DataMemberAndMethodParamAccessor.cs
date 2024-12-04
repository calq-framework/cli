using CalqFramework.Serialization.DataAccess;

namespace CalqFramework.Cli.DataAccess {
    internal class DataMemberAndMethodParamStore : DualKeyValueStore<string,  object?> {
        public ICliDataMemberStore DataMemberStore { get; }
        public CliMethodParamStore MethodParamsStore { get; }
        public object ParentObj { get; }

        //        public ParameterInfo[] Parameters { get => MethodParamsStore.Parameters; }

        public DataMemberAndMethodParamStore(ICliDataMemberStore dataMemberStore, CliMethodParamStore methodParamsStore, object parentObj) : base(dataMemberStore, methodParamsStore)
        {
            DataMemberStore = dataMemberStore;
            MethodParamsStore = methodParamsStore;
            ParentObj = parentObj;
        }

        public object? Invoke()
        {
            return MethodParamsStore.Invoke(ParentObj);
        }
    }
}