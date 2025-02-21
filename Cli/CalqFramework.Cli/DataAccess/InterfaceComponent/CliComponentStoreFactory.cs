using System;
using System.Reflection;
using CalqFramework.Cli.DataAccess.ClassMember;
using CalqFramework.Cli.Serialization;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {

    public class CliComponentStoreFactory : ICliComponentStoreFactory {
        public const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
        private BindingFlags? _methodBindingFlags = null;

        public CliComponentStoreFactory() {
            AccessFields = true;
            BindingFlags = DefaultLookup | BindingFlags.IgnoreCase;
            ClassMemberStringifier = new ClassMemberStringifier();
            ValueConverter = new ValueConverter();
        }

        public bool AccessFields { get; init; } = false;
        public bool AccessProperties { get; init; } = true;
        public BindingFlags BindingFlags { get; init; } = DefaultLookup;

        public IClassMemberStringifier ClassMemberStringifier { get; }

        public BindingFlags MethodBindingFlags {
            get => _methodBindingFlags == null ? BindingFlags : (BindingFlags)_methodBindingFlags;
            init => _methodBindingFlags = value;
        }
        public IValueConverter<string?> ValueConverter { get; init; }

        public IOptionStore CreateOptionStore(object obj) {
            var cliValidator = new OptionAccessorValidator();
            ICliKeyValueStore<string, string?, MemberInfo> store;
            if (AccessFields && AccessProperties) {
                store = CreateFieldAndPropertyStore(obj, cliValidator, ValueConverter);
            } else if (AccessFields) {
                store = CreateFieldStore(obj, cliValidator, ValueConverter);
            } else if (AccessProperties) {
                store = CreatePropertyStore(obj, cliValidator, ValueConverter);
            } else {
                throw new ArgumentException("Neither AccessFields nor AccessProperties is set.");
            }
            return new OptionStore(store);
        }

        public ISubcommandExecutor CreateSubcommandExecutor(MethodInfo method, object? obj) {
            return new SubcommandExecutor(new MethodExecutor<string?>(method, obj, BindingFlags, ClassMemberStringifier, ValueConverter));
        }

        public ISubcommandExecutorWithOptions CreateSubcommandExecutorWithOptions(MethodInfo method, object obj) {
            return new SubcommandExecutorWithOptions(CreateSubcommandExecutor(method, obj), CreateOptionStore(obj));
        }

        public ISubcommandStore CreateSubcommandStore(object obj) {
            return new SubcommandStore(new MethodInfoStore(obj, MethodBindingFlags, ClassMemberStringifier));
        }

        public ISubmoduleStore CreateSubmoduleStore(object obj) {
            var cliValidator = new SubmoduleAccessorValidator();
            var converter = new ReadOnlyPassThroughConverter();
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
            return new FieldStore<TValue>(obj, BindingFlags, ClassMemberStringifier, cliValidator, converter);
        }

        private ICliKeyValueStore<string, TValue, MemberInfo> CreatePropertyStore<TValue>(object obj, IAccessorValidator cliValidator, IValueConverter<TValue> converter) {
            return new PropertyStore<TValue>(obj, BindingFlags, ClassMemberStringifier, cliValidator, converter);
        }
    }
}
