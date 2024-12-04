using CalqFramework.Cli.Serialization;
using CalqFramework.Serialization.DataAccess;
using CalqFramework.Serialization.DataAccess.ClassMember;

namespace CalqFramework.Cli.DataAccess.DataMemberAccess {
    sealed internal class CliClassDataMemberStoreFactory : ClassDataMemberStoreFactoryBase<string, object?>
    {

        public static CliClassDataMemberStoreFactory Instance { get; }

        static CliClassDataMemberStoreFactory()
        {
            Instance = new CliClassDataMemberStoreFactory(new ClassDataMemberStoreFactoryOptions());
        }

        public CliClassDataMemberStoreFactory(ClassDataMemberStoreFactoryOptions classDataMemberStoreFactoryOptions) : base(classDataMemberStoreFactoryOptions)
        {
        }

        public ICliDataMemberStore CreateCliStore(object obj) {
            return (CreateDataMemberStore(obj) as ICliDataMemberStore)!;
        }

        protected override ICliDataMemberStore CreateFieldAndPropertyStore(object obj) {
            return new CliDualKeyValueStore(CreateFieldStore(obj), CreatePropertyStore(obj), new CliDataMemberSerializerFactory(ClassDataMemberStoreFactoryOptions.BindingAttr));
        }

        protected override ICliDataMemberStore CreateFieldStore(object obj) {
            return new CliFieldStore(obj, ClassDataMemberStoreFactoryOptions.BindingAttr, new CliDataMemberSerializerFactory(ClassDataMemberStoreFactoryOptions.BindingAttr));
        }

        protected override ICliDataMemberStore CreatePropertyStore(object obj) {
            return new CliPropertyStore(obj, ClassDataMemberStoreFactoryOptions.BindingAttr, new CliDataMemberSerializerFactory(ClassDataMemberStoreFactoryOptions.BindingAttr));
        }
    }
}
