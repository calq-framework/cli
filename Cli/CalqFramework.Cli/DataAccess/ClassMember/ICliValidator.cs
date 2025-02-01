namespace CalqFramework.Cli.DataAccess.ClassMember {
    public interface ICliValidator {
        bool IsValid(System.Reflection.MemberInfo accessor);
    }
}