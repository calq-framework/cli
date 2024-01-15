using CalqFramework.Serialization.DataAccess;
using CalqFramework.Serialization.DataAccess.DataMemberAccess;

namespace CalqFramework.Options.DataMemberAccess {
    public class DataMemberAndMethodParamsAccessor : DualDataAccessor {
        public DataMemberAccessorBase DataMemberAccessor { get; }
        public MethodParamsAccessor MethodParamsAccessor { get; }

        public DataMemberAndMethodParamsAccessor(DataMemberAccessorBase dataMemberAccessor, MethodParamsAccessor methodParamsAccessor) : base(dataMemberAccessor, methodParamsAccessor) {
            DataMemberAccessor = dataMemberAccessor;
            MethodParamsAccessor = methodParamsAccessor;
        }
    }
}