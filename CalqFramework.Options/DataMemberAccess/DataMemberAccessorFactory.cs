using CalqFramework.Serialization.DataMemberAccess;

namespace CalqFramework.Options.DataMemberAccess {
    sealed public class DataMemberAccessorFactory : DataMemberAccessorFactoryBase {

        public static DataMemberAccessorFactory Instance { get; }

        public static IDataMemberAccessor DefaultDataMemberAccessor { get; }

        static DataMemberAccessorFactory() {
            Instance = new DataMemberAccessorFactory();
            DefaultDataMemberAccessor = Instance.CreateDataMemberAccessor(new DataMemberAccessorOptions() {
                AccessFields = true,
                AccessProperties = true
            });
        }

        protected override IDataMemberAccessor CreateFieldAccessor(DataMemberAccessorOptions dataMemberAccessorOptions) {
            return new FieldAccessor(dataMemberAccessorOptions.BindingAttr);
        }

        protected override IDataMemberAccessor CreatePropertyAccessor(DataMemberAccessorOptions dataMemberAccessorOptions) {
            return new PropertyAccessor(dataMemberAccessorOptions.BindingAttr);
        }
    }
}
