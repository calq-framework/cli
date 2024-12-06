using CalqFramework.Cli.Serialization;
using CalqFramework.DataAccess.ClassMember;
using CalqFramework.DataAccess;
using CalqFramework.Cli.DataAccess.ClassMember;

namespace CalqFramework.Cli.DataAccess {
    sealed internal class CliOptionsStoreFactory : ClassDataMemberStoreFactoryBase<string, object?> {

        public static CliOptionsStoreFactory Instance { get; }

        static CliOptionsStoreFactory() {
            Instance = new CliOptionsStoreFactory(new ClassDataMemberStoreFactoryOptions());
        }

        public CliOptionsStoreFactory(ClassDataMemberStoreFactoryOptions classDataMemberStoreFactoryOptions) : base(classDataMemberStoreFactoryOptions) {
        }

        public ICliOptionsStore CreateCliStore(object obj) {
            return (CreateDataMemberStore(obj) as ICliOptionsStore)!;
        }

        protected override ICliOptionsStore CreateFieldAndPropertyStore(object obj) {
            return new CliDualKeyValueStore(CreateFieldStore(obj), CreatePropertyStore(obj), new CliClassDataMemberSerializerFactory(ClassDataMemberStoreFactoryOptions.BindingAttr));
        }

        protected override ICliOptionsStore CreateFieldStore(object obj) {
            return new CliFieldStore(obj, ClassDataMemberStoreFactoryOptions.BindingAttr, new CliClassDataMemberSerializerFactory(ClassDataMemberStoreFactoryOptions.BindingAttr));
        }

        protected override ICliOptionsStore CreatePropertyStore(object obj) {
            return new CliPropertyStore(obj, ClassDataMemberStoreFactoryOptions.BindingAttr, new CliClassDataMemberSerializerFactory(ClassDataMemberStoreFactoryOptions.BindingAttr));
        }
    }
}
