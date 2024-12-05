using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {
    public class ClassDataMemberStoreFactoryOptions {

        public const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        public bool AccessFields { get; init; } = false;
        public bool AccessProperties { get; init; } = true;

        public BindingFlags BindingAttr { get; init; } = DefaultLookup;

        public ClassDataMemberStoreFactoryOptions() {
        }
    }
}