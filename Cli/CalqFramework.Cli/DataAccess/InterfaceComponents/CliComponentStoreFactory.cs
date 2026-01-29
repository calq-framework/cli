using System;
using System.Reflection;
using CalqFramework.Cli.DataAccess.ClassMembers;
using CalqFramework.Cli.Parsing;
using CalqFramework.Cli.Formatting;
using CalqFramework.DataAccess;
using CalqFramework.DataAccess.Collections;
using CalqFramework.DataAccess.Parsing;

namespace CalqFramework.Cli.DataAccess.InterfaceComponents {

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
            
            ArgValueParser = new ArgValueParser();
            CollectionStoreFactory = new CollectionStoreFactory() { 
                IndexParser = ArgValueParser,
                KeyParser = ArgValueParser 
            };
            ValueConverter = new ValueConverter(CollectionStoreFactory, ArgValueParser);
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
        /// Factory for creating collection stores.
        /// </summary>
        public ICollectionStoreFactory<string, object?> CollectionStoreFactory { get; init; }
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
        /// Defaults to using ArgValueParser if not explicitly set.
        /// </summary>
        public IAccessValidator OptionAccessValidator {
            get => _optionAccessValidator ?? new OptionAccessValidator(ArgValueParser);
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
        /// Defaults to using ArgValueParser if not explicitly set.
        /// </summary>
        public IAccessValidator SubmoduleAccessValidator {
            get => _submoduleAccessValidator ?? new SubmoduleAccessValidator(ArgValueParser);
            init => _submoduleAccessValidator = value;
        }
        /// <summary>
        /// Converter for transforming values between CLI strings and internal types.
        /// </summary>
        public IValueConverter<string?> ValueConverter { get; init; }
        /// <summary>
        /// Parser for converting string argument values to typed objects.
        /// </summary>
        public IStringParser ArgValueParser { get; init; }

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
