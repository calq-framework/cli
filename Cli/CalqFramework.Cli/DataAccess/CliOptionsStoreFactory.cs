using CalqFramework.Cli.Serialization;
using CalqFramework.DataAccess.ClassMember;
using CalqFramework.DataAccess;
using CalqFramework.Cli.DataAccess.ClassMember;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess {
    sealed public class CliOptionsStoreFactory : ClassDataMemberStoreFactoryBase<string, object?> {

        public CliOptionsStoreFactory() {
            AccessFields = true;
            BindingAttr = DefaultLookup | BindingFlags.IgnoreCase;
        }

        public ICliOptionsStore CreateCliStore(object obj) {
            return (CreateDataMemberStore(obj) as ICliOptionsStore)!;
        }

        protected override ICliOptionsStore CreateFieldAndPropertyStore(object obj) {
            return new CliDualKeyValueStore(CreateFieldStore(obj), CreatePropertyStore(obj), new CliClassDataMemberSerializerFactory(BindingAttr));
        }

        protected override ICliOptionsStore CreateFieldStore(object obj) {
            return new CliFieldStore(obj, BindingAttr, new CliClassDataMemberSerializerFactory(BindingAttr));
        }

        protected override ICliOptionsStore CreatePropertyStore(object obj) {
            return new CliPropertyStore(obj, BindingAttr, new CliClassDataMemberSerializerFactory(BindingAttr));
        }
    }
}
