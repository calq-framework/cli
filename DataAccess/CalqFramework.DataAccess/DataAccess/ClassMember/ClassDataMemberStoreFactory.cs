using System.Reflection;

namespace CalqFramework.Serialization.DataAccess.ClassMember;
sealed public class ClassDataMemberStoreFactory : ClassDataMemberStoreFactoryBase<string, object?>
{

    public static ClassDataMemberStoreFactory Instance { get; }

    static ClassDataMemberStoreFactory()
    {
        Instance = new ClassDataMemberStoreFactory(new ClassDataMemberStoreFactoryOptions());
    }

    public ClassDataMemberStoreFactory(ClassDataMemberStoreFactoryOptions classDataMemberStoreOptions) : base(classDataMemberStoreOptions)
    {
    }

    protected override IKeyValueStore<string, object?, MemberInfo> CreateFieldStore(object obj) {
        return ClassDataMemberStoreFactoryOptions.AccessProperties == true ? new FieldStore(obj, ClassDataMemberStoreFactoryOptions.BindingAttr) : new FieldStoreNoValidation(obj, ClassDataMemberStoreFactoryOptions.BindingAttr);
    }

    protected override IKeyValueStore<string, object?, MemberInfo> CreatePropertyStore(object obj) {
        return ClassDataMemberStoreFactoryOptions.AccessFields == true ? new PropertyStore(obj, ClassDataMemberStoreFactoryOptions.BindingAttr) : new PropertyStoreNoValidation(obj, ClassDataMemberStoreFactoryOptions.BindingAttr); 
    }
}
