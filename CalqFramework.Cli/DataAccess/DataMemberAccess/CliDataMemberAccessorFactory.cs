using CalqFramework.Serialization.DataAccess;
using CalqFramework.Serialization.DataAccess.DataMemberAccess;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.DataMemberAccess {
    sealed internal class CliDataMemberAccessorFactory : DataMemberAccessorFactoryBase<string, object?>
    {

        public static CliDataMemberAccessorFactory Instance { get; }

        static CliDataMemberAccessorFactory()
        {
            Instance = new CliDataMemberAccessorFactory(new DataMemberAccessorOptions());
        }

        public CliDataMemberAccessorFactory(DataMemberAccessorOptions dataMemberAccessorOptions) : base(dataMemberAccessorOptions)
        {
        }

        protected override IDataAccessor<string, object?, MemberInfo> CreateFieldAndPropertyAccessor(object obj) {
            return new CliDualDataAccessor(CreateFieldAccessor(obj), CreatePropertyAccessor(obj));
        }

        protected override IDataAccessor<string, object?, MemberInfo> CreateFieldAccessor(object obj) {
            return new CliFieldAccessor(obj, DataMemberAccessorOptions.BindingAttr);
        }

        protected override IDataAccessor<string, object?, MemberInfo> CreatePropertyAccessor(object obj) {
            return new CliPropertyAccessor(obj, DataMemberAccessorOptions.BindingAttr);
        }
    }
}
