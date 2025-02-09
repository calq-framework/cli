using CalqFramework.Cli.Serialization;
using CalqFramework.Cli.DataAccess.ClassMember;
using System.Reflection;
using System;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    public class CliComponentStoreFactory : ICliComponentStoreFactory {
        private BindingFlags? _methodBindingAttr = null;

        internal IClassMemberSerializer CliClassDataMemberSerializer { get; }

        public const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        public bool AccessFields { get; init; } = false;
        public bool AccessProperties { get; init; } = true;

        public BindingFlags MethodBindingAttr {
            get => _methodBindingAttr == null ? BindingAttr : (BindingFlags)_methodBindingAttr;
            init => _methodBindingAttr = value;
        }

        public BindingFlags BindingAttr { get; init; } = DefaultLookup;

        public CliComponentStoreFactory() {
            AccessFields = true;
            BindingAttr = DefaultLookup | BindingFlags.IgnoreCase;
            CliClassDataMemberSerializer = new ClassMemberSerializer();
        }

        public IOptionStore CreateOptionStore(object obj) {
            var cliValidator = new OptionAccessorValidator();
            var converter = new OptionCnverter();
            ICliStore<string, string?, MemberInfo> store;
            if (AccessFields && AccessProperties) {
                store = CreateFieldAndPropertyStore(obj, cliValidator, converter);
            } else if (AccessFields) {
                store = CreateFieldStore(obj, cliValidator, converter);
            } else if (AccessProperties) {
                store = CreatePropertyStore(obj, cliValidator, converter);
            } else {
                throw new ArgumentException("Neither AccessFields nor AccessProperties is set.");
            }
            return new OptionStore(store);
        }

        public ISubmoduleStore CreateSubmoduleStore(object obj) {
            var cliValidator = new SubmoduleAccessorValidator();
            var converter = new SubmoduleCnverter();
            ICliStore<string, object?, MemberInfo> store;
            if (AccessFields && AccessProperties) {
                store = CreateFieldAndPropertyStore(obj, cliValidator, converter);
            } else if (AccessFields) {
                store = CreateFieldStore(obj, cliValidator, converter);
            } else if (AccessProperties) {
                store = CreatePropertyStore(obj, cliValidator, converter);
            } else {
                throw new ArgumentException("Neither AccessFields nor AccessProperties is set.");
            }
            return new SubmoduleStore(store);
        }

        private ICliStore<string, TValue, MemberInfo> CreateFieldAndPropertyStore<TValue>(object obj, IAccessorValidator cliValidator, IValueConverter<TValue> converter) {
            return new CliDualKeyValueStore<TValue>(CreateFieldStore(obj, cliValidator, converter), CreatePropertyStore(obj, cliValidator, converter), BindingAttr, CliClassDataMemberSerializer);
        }

        private ICliStore<string, TValue, MemberInfo> CreateFieldStore<TValue>(object obj, IAccessorValidator cliValidator, IValueConverter<TValue> converter) {
            return new CliFieldStore<TValue>(obj, BindingAttr, CliClassDataMemberSerializer, cliValidator, converter);
        }

        private ICliStore<string, TValue, MemberInfo> CreatePropertyStore<TValue>(object obj, IAccessorValidator cliValidator, IValueConverter<TValue> converter) {
            return new CliPropertyStore<TValue>(obj, BindingAttr, CliClassDataMemberSerializer, cliValidator, converter);
        }

        public MethodResolver CreateMethodResolver(object targetObj) {
            return new MethodResolver(targetObj, MethodBindingAttr);
        }
    }
}
