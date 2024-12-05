using CalqFramework.Serialization.DataAccess;

namespace CalqFramework.Cli.DataAccess {
    internal class ClassDataMemberAndMethodParamStore : DualKeyValueStore<string,  object?> {
        public ICliKeyValueStore ClassDataMemberStore { get; }
        public CliMethodParamStore MethodParamsStore { get; }
        public object ParentObj { get; }

        //        public ParameterInfo[] Parameters { get => MethodParamsStore.Parameters; }

        public ClassDataMemberAndMethodParamStore(ICliKeyValueStore classDataMemberStore, CliMethodParamStore methodParamsStore, object parentObj) : base(classDataMemberStore, methodParamsStore)
        {
            ClassDataMemberStore = classDataMemberStore;
            MethodParamsStore = methodParamsStore;
            ParentObj = parentObj;
        }

        public object? Invoke()
        {
            return MethodParamsStore.Invoke(ParentObj);
        }
    }
}