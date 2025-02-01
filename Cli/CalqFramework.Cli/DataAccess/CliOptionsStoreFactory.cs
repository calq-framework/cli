using CalqFramework.Cli.Serialization;
using CalqFramework.Cli.DataAccess.ClassMember;
using System.Reflection;
using System;

namespace CalqFramework.Cli.DataAccess {
    public class CliOptionsStoreFactory : ICliOptionsStoreFactory {

        internal ICliClassDataMemberSerializer CliClassDataMemberSerializer { get; }

        public const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        public bool AccessFields { get; init; } = false;
        public bool AccessProperties { get; init; } = true;

        public BindingFlags BindingAttr { get; init; } = DefaultLookup;

        public CliOptionsStoreFactory() {
            AccessFields = true;
            BindingAttr = DefaultLookup | BindingFlags.IgnoreCase;
            CliClassDataMemberSerializer = new CliClassDataMemberSerializer();
        }

        public ICliOptionsStore<string, object?, MemberInfo> CreateOptonStore(object obj) {
            var cliValidator = new CliOptionValidator();
            ICliStore<string, object?, MemberInfo> store;
            if (AccessFields && AccessProperties) {
                store = CreateFieldAndPropertyStore(obj, cliValidator);
            } else if (AccessFields) {
                store = CreateFieldStore(obj, cliValidator);
            } else if (AccessProperties) {
                store = CreatePropertyStore(obj, cliValidator);
            } else {
                throw new ArgumentException("Neither AccessFields nor AccessProperties is set.");
            }
            return new CliOptionsStore<string, object?, MemberInfo>(store);
        }

        public ICliCommandStore<string, object?, MemberInfo> CreateCommandStore(object obj) {
            var cliValidator = new CliCommandValidator();
            ICliStore<string, object?, MemberInfo> store;
            if (AccessFields && AccessProperties) {
                store = CreateFieldAndPropertyStore(obj, cliValidator);
            } else if (AccessFields) {
                store = CreateFieldStore(obj, cliValidator);
            } else if (AccessProperties) {
                store = CreatePropertyStore(obj, cliValidator);
            } else {
                throw new ArgumentException("Neither AccessFields nor AccessProperties is set.");
            }
            return new CliCommandStore<string, object?, MemberInfo>(store);
        }

        protected ICliStore<string, object?, MemberInfo> CreateFieldAndPropertyStore(object obj, ICliValidator cliValidator) {
            return new CliDualKeyValueStore(CreateFieldStore(obj, cliValidator), CreatePropertyStore(obj, cliValidator), BindingAttr, CliClassDataMemberSerializer);
        }

        protected ICliStore<string, object?, MemberInfo> CreateFieldStore(object obj, ICliValidator cliValidator) {
            return new CliFieldStore(obj, BindingAttr, CliClassDataMemberSerializer, cliValidator);
        }

        protected ICliStore<string, object?, MemberInfo> CreatePropertyStore(object obj, ICliValidator cliValidator) {
            return new CliPropertyStore(obj, BindingAttr, CliClassDataMemberSerializer, cliValidator);
        }
    }
}
