namespace CalqFramework.Cli.Serialization {
    public interface ICliClassDataMemberSerializer {
        string GetOptionsString();

        string GetCommandsString();
    }
}