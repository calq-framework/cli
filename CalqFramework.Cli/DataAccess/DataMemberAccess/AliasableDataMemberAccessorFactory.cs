using CalqFramework.Serialization.DataAccess.DataMemberAccess;

namespace CalqFramework.Cli.DataAccess.DataMemberAccess
{
    sealed internal class AliasableDataMemberAccessorFactory : DataMemberAccessorFactoryBase
    {

        public static AliasableDataMemberAccessorFactory Instance { get; }

        static AliasableDataMemberAccessorFactory()
        {
            Instance = new AliasableDataMemberAccessorFactory(new DataMemberAccessorOptions());
        }

        public AliasableDataMemberAccessorFactory(DataMemberAccessorOptions dataMemberAccessorOptions) : base(dataMemberAccessorOptions)
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
