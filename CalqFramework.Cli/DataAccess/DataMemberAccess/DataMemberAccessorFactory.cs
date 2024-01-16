using CalqFramework.Serialization.DataAccess.DataMemberAccess;

namespace CalqFramework.Cli.DataAccess.DataMemberAccess
{
    sealed internal class DataMemberAccessorFactory : DataMemberAccessorFactoryBase
    {

        public static DataMemberAccessorFactory Instance { get; }

        static DataMemberAccessorFactory()
        {
            Instance = new DataMemberAccessorFactory(new DataMemberAccessorOptions());
        }

        public DataMemberAccessorFactory(DataMemberAccessorOptions dataMemberAccessorOptions) : base(dataMemberAccessorOptions)
        {
        }

        protected override FieldAccessorBase CreateFieldAccessor(object obj)
        {
            return new AliasableFieldAccessor(obj, DataMemberAccessorOptions.BindingAttr);
        }
        protected override PropertyAccessorBase CreatePropertyAccessor(object obj)
        {
            return new AliasablePropertyAccessor(obj, DataMemberAccessorOptions.BindingAttr);
        }
    }
}
