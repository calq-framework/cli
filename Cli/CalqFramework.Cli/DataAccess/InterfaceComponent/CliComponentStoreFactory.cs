using System;
using System.Reflection;
using CalqFramework.Cli.DataAccess.ClassMember;
using CalqFramework.Cli.Serialization;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {

    /// <summary>
    /// Factory for creating CLI component stores with configurable behavior.
    /// </summary>
    public class CliComponentStoreFactory : ICliComponentStoreFactory {
        /// <summary>
        /// Default binding flags for member lookup (Instance, Static, Public, IgnoreCase).
        /// </summary>
        public const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase;
        private BindingFlags? _methodBindingFlags = null;

        public CliComponentStoreFactory() {
            AccessFields = true;
            AccessProperties = true;
            BindingFlags = DefaultLookup;
            ClassMemberStringifier = new ClassMemberStringifier();
            EnableShadowing = false;
            ValueConverter = new ValueConverter();
            OptionAccessValidator = new OptionAccessValidator();
            SubmoduleAccessValidator = new SubmoduleAccessValidator();
            SubcommandAccessValidator = new SubcommandAccessValidator();
        }

        /// <summary>
        /// Access fields as CLI options.
        /// </summary>
        public bool AccessFields { get; init; }
        /// <summary>
        /// Access properties as CLI options.
        /// </summary>
        public bool AccessProperties { get; init; }
        /// <summary>
        /// Binding flags for member lookup.
        /// </summary>
        public BindingFlags BindingFlags { get; init; }
        /// <summary>
        /// Stringifier for converting member names to CLI format.
        /// </summary>
        public IClassMemberStringifier ClassMemberStringifier { get; init; }
        /// <summary>
        /// Method parameters can shadow fields and properties.
        /// </summary>
        public bool EnableShadowing { get; init; }
        /// <summary>
        /// Binding flags for method lookup (defaults to BindingFlags if not set).
        /// </summary>
        public BindingFlags MethodBindingFlags {
            get => _methodBindingFlags == null ? BindingFlags : (BindingFlags)_methodBindingFlags;
            init => _methodBindingFlags = value;
        }
        /// <summary>
        /// Validator for determining which members are valid options.
        /// </summary>
        public IAccessValidator OptionAccessValidator { get; init; }
        /// <summary>
        /// Validator for determining which methods are valid subcommands.
        /// </summary>
        public IAccessValidator SubcommandAccessValidator { get; init; }
        /// <summary>
        /// Validator for determining which members are valid submodules.
        /// </summary>
        public IAccessValidator SubmoduleAccessValidator { get; init; }
        /// <summary>
        /// Converter for transforming values between CLI strings and internal types.
        /// </summary>
        public IValueConverter<string?> ValueConverter { get; init; }
        public IOptionStore CreateOptionStore(object obj) {
            ICliKeyValueStore<string, string?, MemberInfo> store;
            if (AccessFields && AccessProperties) {
                store = CreateFieldAndPropertyStore(obj, OptionAccessValidator, ValueConverter);
            } else if (AccessFields) {
                store = CreateFieldStore(obj, OptionAccessValidator, ValueConverter);
            } else if (AccessProperties) {
                store = CreatePropertyStore(obj, OptionAccessValidator, ValueConverter);
            } else {
                throw new ArgumentException("Neither AccessFields nor AccessProperties is set.");
            }
            return new OptionStore(store);
        }

        public ISubcommandExecutor CreateSubcommandExecutor(MethodInfo method, object? obj) {
            return new SubcommandExecutor(new MethodExecutor<string?>(method, obj, BindingFlags, ClassMemberStringifier, ValueConverter));
        }

        public ISubcommandExecutorWithOptions CreateSubcommandExecutorWithOptions(MethodInfo method, object obj) {
            return new SubcommandExecutorWithOptions(CreateSubcommandExecutor(method, obj), CreateOptionStore(obj)) { EnableShadowing = EnableShadowing };
        }

        public ISubcommandStore CreateSubcommandStore(object obj) {
            return new SubcommandStore(new MethodInfoStore(obj, MethodBindingFlags, ClassMemberStringifier, SubcommandAccessValidator));
        }

        public ISubmoduleStore CreateSubmoduleStore(object obj) {
            var converter = new ReadOnlyPassThroughConverter();
            ICliKeyValueStore<string, object?, MemberInfo> store;
            if (AccessFields && AccessProperties) {
                store = CreateFieldAndPropertyStore(obj, SubmoduleAccessValidator, converter);
            } else if (AccessFields) {
                store = CreateFieldStore(obj, SubmoduleAccessValidator, converter);
            } else if (AccessProperties) {
                store = CreatePropertyStore(obj, SubmoduleAccessValidator, converter);
            } else {
                throw new ArgumentException("Neither AccessFields nor AccessProperties is set.");
            }
            return new SubmoduleStore(store);
        }

        private ICliKeyValueStore<string, TValue, MemberInfo> CreateFieldAndPropertyStore<TValue>(object obj, IAccessValidator cliValidator, IValueConverter<TValue> converter) {
            return new CliDualKeyValueStore<TValue>(CreateFieldStore(obj, cliValidator, converter), CreatePropertyStore(obj, cliValidator, converter));
        }

        private ICliKeyValueStore<string, TValue, MemberInfo> CreateFieldStore<TValue>(object obj, IAccessValidator cliValidator, IValueConverter<TValue> converter) {
            return new FieldStore<TValue>(obj, BindingFlags, ClassMemberStringifier, cliValidator, converter);
        }

        private ICliKeyValueStore<string, TValue, MemberInfo> CreatePropertyStore<TValue>(object obj, IAccessValidator cliValidator, IValueConverter<TValue> converter) {
            return new PropertyStore<TValue>(obj, BindingFlags, ClassMemberStringifier, cliValidator, converter);
        }
    }
}
