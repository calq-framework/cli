using CalqFramework.Cli.Serialization;
using CalqFramework.DataAccess.ClassMember;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess.ClassMember {
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

        public ICliKeyValueStore CreateCliStore(object obj) {
            return (CreateDataMemberStore(obj) as ICliKeyValueStore)!;
        }

        protected override ICliKeyValueStore CreateFieldAndPropertyStore(object obj) {
            return new CliDualKeyValueStore(CreateFieldStore(obj), CreatePropertyStore(obj), new CliClassDataMemberSerializerFactory(ClassDataMemberStoreFactoryOptions.BindingAttr));
        }

        protected override ICliKeyValueStore CreateFieldStore(object obj) {
            return new CliFieldStore(obj, ClassDataMemberStoreFactoryOptions.BindingAttr, new CliClassDataMemberSerializerFactory(ClassDataMemberStoreFactoryOptions.BindingAttr));
        }

        protected override ICliKeyValueStore CreatePropertyStore(object obj) {
            return new CliPropertyStore(obj, ClassDataMemberStoreFactoryOptions.BindingAttr, new CliClassDataMemberSerializerFactory(ClassDataMemberStoreFactoryOptions.BindingAttr));
        }
    }
}
