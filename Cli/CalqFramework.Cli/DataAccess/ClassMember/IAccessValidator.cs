namespace CalqFramework.Cli.DataAccess.ClassMember {

    public interface IAccessValidator {

        bool IsValid(System.Reflection.MemberInfo accessor);
    }
}
