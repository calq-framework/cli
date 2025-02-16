using CalqFramework.Cli.DataAccess.ClassMember;
using CalqFramework.Cli.Serialization;
using System;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {

    public class CliComponentStoreFactory : ICliComponentStoreFactory {
        public const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
        private BindingFlags? _methodBindingAttr = null;

        public CliComponentStoreFactory() {
            AccessFields = true;
            BindingAttr = DefaultLookup | BindingFlags.IgnoreCase;
            CliClassDataMemberSerializer = new ClassMemberSerializer();
        }

        public bool AccessFields { get; init; } = false;
        public bool AccessProperties { get; init; } = true;
        public BindingFlags BindingAttr { get; init; } = DefaultLookup;

        public BindingFlags MethodBindingAttr {
            get => _methodBindingAttr == null ? BindingAttr : (BindingFlags)_methodBindingAttr;
            init => _methodBindingAttr = value;
        }

        internal IClassMemberSerializer CliClassDataMemberSerializer { get; }

        public IOptionStore CreateOptionStore(object obj) {
            var cliValidator = new OptionAccessorValidator();
            var converter = new OptionConverter();
            ICliKeyValueStore<string, string?, MemberInfo> store;
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

        public ISubcommandExecutor CreateSubcommandExecutor(MethodInfo methodInfo, object? obj) {
            return new SubcommandExecutor(new MethodExecutor(methodInfo, obj));
        }

        public ISubcommandExecutorWithOptions CreateSubcommandExecutorWithOptions(MethodInfo cliAction, object obj) {
            return new SubcommandExecutorWithOptions(CreateSubcommandExecutor(cliAction, obj), CreateOptionStore(obj));
        }

        public ISubcommandStore CreateSubcommandStore(object obj) {
            return new SubcommandStore(new MethodInfoStore(obj, MethodBindingAttr));
        }

        public ISubmoduleStore CreateSubmoduleStore(object obj) {
            var cliValidator = new SubmoduleAccessorValidator();
            var converter = new SubmoduleConverter();
            ICliKeyValueStore<string, object?, MemberInfo> store;
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

        private ICliKeyValueStore<string, TValue, MemberInfo> CreateFieldAndPropertyStore<TValue>(object obj, IAccessorValidator cliValidator, IValueConverter<TValue> converter) {
            return new CliDualKeyValueStore<TValue>(CreateFieldStore(obj, cliValidator, converter), CreatePropertyStore(obj, cliValidator, converter));
        }

        private ICliKeyValueStore<string, TValue, MemberInfo> CreateFieldStore<TValue>(object obj, IAccessorValidator cliValidator, IValueConverter<TValue> converter) {
            return new FieldStore<TValue>(obj, BindingAttr, CliClassDataMemberSerializer, cliValidator, converter);
        }

        private ICliKeyValueStore<string, TValue, MemberInfo> CreatePropertyStore<TValue>(object obj, IAccessorValidator cliValidator, IValueConverter<TValue> converter) {
            return new PropertyStore<TValue>(obj, BindingAttr, CliClassDataMemberSerializer, cliValidator, converter);
        }
    }
}