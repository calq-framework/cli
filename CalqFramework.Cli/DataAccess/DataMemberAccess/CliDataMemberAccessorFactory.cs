using CalqFramework.Cli.Serialization;
using CalqFramework.Serialization.DataAccess;
using CalqFramework.Serialization.DataAccess.DataMemberAccess;

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

        public ICliDataMemberAccessor CreateCliAccessor(object obj) {
            return (CreateDataMemberAccessor(obj) as ICliDataMemberAccessor)!;
        }

        protected override ICliDataMemberAccessor CreateFieldAndPropertyAccessor(object obj) {
            return new CliDualDataAccessor(CreateFieldAccessor(obj), CreatePropertyAccessor(obj), new CliDataMemberSerializerFactory(DataMemberAccessorOptions.BindingAttr));
        }

        protected override ICliDataMemberAccessor CreateFieldAccessor(object obj) {
            return new CliFieldAccessor(obj, DataMemberAccessorOptions.BindingAttr, new CliDataMemberSerializerFactory(DataMemberAccessorOptions.BindingAttr));
        }

        protected override ICliDataMemberAccessor CreatePropertyAccessor(object obj) {
            return new CliPropertyAccessor(obj, DataMemberAccessorOptions.BindingAttr, new CliDataMemberSerializerFactory(DataMemberAccessorOptions.BindingAttr));
        }
    }
}
