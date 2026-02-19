using System;
using System.Reflection;
using CalqFramework.Cli.DataAccess.ClassMemberStores;
using CalqFramework.Cli.DataAccess.InterfaceComponentStores;
using CalqFramework.Cli.Formatting;
using CalqFramework.DataAccess;
using CalqFramework.DataAccess.ClassMemberStores;
using CalqFramework.DataAccess.CollectionElementStores;

namespace CalqFramework.Cli.DataAccess {

    /// <summary>
    /// Factory for creating CLI component stores with configurable behavior.
    /// </summary>
    public class CliComponentStoreFactory : ICliComponentStoreFactory {
        /// <summary>
        /// Default binding flags for member lookup (Instance, Static, Public, IgnoreCase).
        /// </summary>
        public const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase;
        private BindingFlags? _methodBindingFlags = null;
        private IAccessValidator? _optionAccessValidator = null;
        private IAccessValidator? _subcommandAccessValidator = null;
        private IAccessValidator? _submoduleAccessValidator = null;

        public CliComponentStoreFactory() {
            AccessFields = true;
            AccessProperties = true;
            BindingFlags = DefaultLookup;
            ClassMemberStringifier = new ClassMemberStringifier();
            EnableShadowing = false;
        }

        private ICollectionElementStoreFactory<string, object?>? _collectionElementStoreFactory;
        private ICompositeValueConverter<string?>? _compositeValueConverter;

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
        /// Culture to use for parsing argument values. Defaults to InvariantCulture.
        /// </summary>
        public IFormatProvider FormatProvider { get; init; } = System.Globalization.CultureInfo.InvariantCulture;
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
        /// Defaults to using CompositeValueConverter if not explicitly set.
        /// </summary>
        public IAccessValidator OptionAccessValidator {
            get => _optionAccessValidator ?? new OptionAccessValidator(CompositeValueConverter);
            init => _optionAccessValidator = value;
        }
        /// <summary>
        /// Validator for determining which methods are valid subcommands.
        /// Defaults to SubcommandAccessValidator if not explicitly set.
        /// </summary>
        public IAccessValidator SubcommandAccessValidator {
            get => _subcommandAccessValidator ?? new SubcommandAccessValidator();
            init => _subcommandAccessValidator = value;
        }
        /// <summary>
        /// Validator for determining which members are valid submodules.
        /// Defaults to using CompositeValueConverter if not explicitly set.
        /// </summary>
        public IAccessValidator SubmoduleAccessValidator {
            get => _submoduleAccessValidator ?? new SubmoduleAccessValidator(CompositeValueConverter);
            init => _submoduleAccessValidator = value;
        }
        /// <summary>
        /// Factory for creating collection stores.
        /// </summary>
        public ICollectionElementStoreFactory<string, object?> CollectionStoreFactory { 
            get => _collectionElementStoreFactory ??= new CollectionElementStoreFactory();
            init => _collectionElementStoreFactory = value;
        }
        /// <summary>
        /// Converter for transforming values between CLI strings and internal types.
        /// Defaults to CompositeValueConverter wrapping ValueConverter with collection support.
        /// </summary>
        public ICompositeValueConverter<string?> CompositeValueConverter { 
            get {
                if (_compositeValueConverter == null) {
                    var baseConverter = new ValueConverter() { FormatProvider = FormatProvider };
                    _compositeValueConverter = new CompositeValueConverter(baseConverter, CollectionStoreFactory);
                }
                return _compositeValueConverter;
            }
            init => _compositeValueConverter = value;
        }

        public IOptionStore CreateOptionStore(object obj) {
            ICliKeyValueStore<string, string?, MemberInfo> store;
            if (AccessFields && AccessProperties) {
                store = CreateFieldAndPropertyStore(obj, OptionAccessValidator, CompositeValueConverter);
            } else if (AccessFields) {
                store = CreateFieldStore(obj, OptionAccessValidator, CompositeValueConverter);
            } else if (AccessProperties) {
                store = CreatePropertyStore(obj, OptionAccessValidator, CompositeValueConverter);
            } else {
                throw DataAccessErrors.NoAccessConfigured();
            }
            return new OptionStore(store);
        }

        public ISubcommandExecutor CreateSubcommandExecutor(MethodInfo method, object? obj) {
            return new SubcommandExecutor(new CliMethodExecutor<string?>(method, obj, BindingFlags, ClassMemberStringifier, CompositeValueConverter));
        }

        public ISubcommandExecutorWithOptions CreateSubcommandExecutorWithOptions(MethodInfo method, object obj) {
            return new SubcommandExecutorWithOptions(CreateSubcommandExecutor(method, obj), CreateOptionStore(obj)) { EnableShadowing = EnableShadowing };
        }

        public ISubcommandStore CreateSubcommandStore(object obj) {
            return new SubcommandStore(new MethodInfoStore(obj, MethodBindingFlags, ClassMemberStringifier, SubcommandAccessValidator));
        }

        public ISubmoduleStore CreateSubmoduleStore(object obj) {
            var converter = new ReadOnlyPassThroughConverter<object?>();
            ICliKeyValueStore<string, object?, MemberInfo> store;
            if (AccessFields && AccessProperties) {
                store = CreateFieldAndPropertyStore(obj, SubmoduleAccessValidator, converter);
            } else if (AccessFields) {
                store = CreateFieldStore(obj, SubmoduleAccessValidator, converter);
            } else if (AccessProperties) {
                store = CreatePropertyStore(obj, SubmoduleAccessValidator, converter);
            } else {
                throw DataAccessErrors.NoAccessConfigured();
            }
            return new SubmoduleStore(store);
        }

        private ICliKeyValueStore<string, TValue, MemberInfo> CreateFieldAndPropertyStore<TValue>(object obj, IAccessValidator cliValidator, ICompositeValueConverter<TValue> compositeValueConverter) {
            return new CliDualKeyValueStore<TValue>(CreateFieldStore(obj, cliValidator, compositeValueConverter), CreatePropertyStore(obj, cliValidator, compositeValueConverter));
        }

        private ICliKeyValueStore<string, TValue, MemberInfo> CreateFieldStore<TValue>(object obj, IAccessValidator cliValidator, ICompositeValueConverter<TValue> compositeValueConverter) {
            return new CliFieldStore<TValue>(obj, BindingFlags, ClassMemberStringifier, cliValidator, compositeValueConverter);
        }

        private ICliKeyValueStore<string, TValue, MemberInfo> CreatePropertyStore<TValue>(object obj, IAccessValidator cliValidator, ICompositeValueConverter<TValue> compositeValueConverter) {
            return new CliPropertyStore<TValue>(obj, BindingFlags, ClassMemberStringifier, cliValidator, compositeValueConverter);
        }
    }
}
