using CalqFramework.Serialization.DataMemberAccess;

namespace CalqFramework.Options.DataMemberAccess {
    sealed public class CliDataMemberAccessorFactory : DataMemberAccessorFactoryBase {

        public static CliDataMemberAccessorFactory Instance { get; }

        public static IDataMemberAccessor DefaultDataMemberAccessor { get; }

        static CliDataMemberAccessorFactory() {
            Instance = new CliDataMemberAccessorFactory();
            DefaultDataMemberAccessor = Instance.CreateDataMemberAccessor(new CliSerializerOptions());
        }

        protected override IDataMemberAccessor CreateFieldAccessor(DataMemberAccessorOptions dataMemberAccessorOptions) {
            return new CliFieldAccessor(dataMemberAccessorOptions.BindingAttr);
        }

        protected override IDataMemberAccessor CreatePropertyAccessor(DataMemberAccessorOptions dataMemberAccessorOptions) {
            return new CliPropertyAccessor(dataMemberAccessorOptions.BindingAttr);
        }
    }
}
