using CalqFramework.Serialization.DataAccess.ClassMember;
using System.Reflection;

namespace CalqFramework.Cli.Serialization {
    public class CliDeserializerOptions
    {
        private BindingFlags? _methodBindingAttr = null;

        public ClassDataMemberStoreFactoryOptions ClassDataMemberStoreFactoryOptions { get; init; }
        public bool SkipUnknown { get; init; } = false;
        public BindingFlags MethodBindingAttr
        {
            get => _methodBindingAttr == null ? ClassDataMemberStoreFactoryOptions.BindingAttr : (BindingFlags)_methodBindingAttr;
            init => _methodBindingAttr = value;
        }
        public bool UseRevisionVersion { get; init; } = true;

        public CliDeserializerOptions()
        {
            ClassDataMemberStoreFactoryOptions = new ClassDataMemberStoreFactoryOptions {
                AccessFields = true,
                BindingAttr = ClassDataMemberStoreFactoryOptions.DefaultLookup | BindingFlags.IgnoreCase
            };
        }
    }
}
