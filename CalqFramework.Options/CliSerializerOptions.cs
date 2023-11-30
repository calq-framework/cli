using CalqFramework.Serialization.DataMemberAccess;

namespace CalqFramework.Options {
    public class CliSerializerOptions : DataMemberAccessorOptions {
        public bool SkipUnknown { get; init; } = false;

        public CliSerializerOptions() {
            AccessFields = true;
        }
    }
}
