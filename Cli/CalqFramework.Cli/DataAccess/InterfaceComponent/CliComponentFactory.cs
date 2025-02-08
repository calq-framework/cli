using CalqFramework.Cli.Serialization;
using CalqFramework.Cli.DataAccess.ClassMember;
using System.Reflection;
using System;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    public class CliComponentFactory : ICliComponentFactory {

        internal IClassMemberSerializer CliClassDataMemberSerializer { get; }

        public const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        public bool AccessFields { get; init; } = false;
        public bool AccessProperties { get; init; } = true;

        public BindingFlags BindingAttr { get; init; } = DefaultLookup;

        public CliComponentFactory() {
            AccessFields = true;
            BindingAttr = DefaultLookup | BindingFlags.IgnoreCase;
            CliClassDataMemberSerializer = new ClassMemberSerializer();
        }

        public IOptionStore<string, object?, MemberInfo> CreateOptionStore(object obj) {
            var cliValidator = new OptionAccessorValidator();
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
            return new OptionStore<string, object?, MemberInfo>(store);
        }

        public ISubmoduleStore<string, object?, MemberInfo> CreateSubmoduleStore(object obj) {
            var cliValidator = new SubmoduleAccessorValidator();
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
            return new SubmoduleStore<string, object?, MemberInfo>(store);
        }

        protected ICliStore<string, object?, MemberInfo> CreateFieldAndPropertyStore(object obj, IAccessorValidator cliValidator) {
            return new CliDualKeyValueStore(CreateFieldStore(obj, cliValidator), CreatePropertyStore(obj, cliValidator), BindingAttr, CliClassDataMemberSerializer);
        }

        protected ICliStore<string, object?, MemberInfo> CreateFieldStore(object obj, IAccessorValidator cliValidator) {
            return new CliFieldStore(obj, BindingAttr, CliClassDataMemberSerializer, cliValidator);
        }

        protected ICliStore<string, object?, MemberInfo> CreatePropertyStore(object obj, IAccessorValidator cliValidator) {
            return new CliPropertyStore(obj, BindingAttr, CliClassDataMemberSerializer, cliValidator);
        }
    }
}
