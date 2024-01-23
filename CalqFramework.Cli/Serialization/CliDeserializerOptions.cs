using CalqFramework.Serialization.DataAccess.DataMemberAccess;
using System.Reflection;

namespace CalqFramework.Cli.Serialization {
    public class CliDeserializerOptions
    {
        private BindingFlags? _methodBindingAttr = null;

        public DataMemberAccessorOptions DataMemberAccessorOptions { get; init; }
        public bool SkipUnknown { get; init; } = false;
        public BindingFlags MethodBindingAttr
        {
            get => _methodBindingAttr == null ? DataMemberAccessorOptions.BindingAttr : (BindingFlags)_methodBindingAttr;
            init => _methodBindingAttr = value;
        }
        public bool UseRevisionVersion { get; init; } = true;

        public CliDeserializerOptions()
        {
            DataMemberAccessorOptions = new DataMemberAccessorOptions {
                AccessFields = true,
                BindingAttr = DataMemberAccessorOptions.DefaultLookup | BindingFlags.IgnoreCase
            };
        }
    }
}
