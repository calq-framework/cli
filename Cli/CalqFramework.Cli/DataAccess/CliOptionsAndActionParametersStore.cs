using CalqFramework.Cli.Serialization.Parsing;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess {
    internal class CliOptionsAndActionParametersStore : DualKeyValueStore<string,  object?> {
        public ICliOptionsStore Options { get; }
        public CliActionParametersStore ActionParameters { get; }
        public object ParentObj { get; }

        //        public ParameterInfo[] Parameters { get => ActionParameters.Parameters; }

        public CliOptionsAndActionParametersStore(ICliOptionsStore options, CliActionParametersStore actionParameters, object parentObj) : base(options, actionParameters)
        {
            Options = options;
            ActionParameters = actionParameters;
            ParentObj = parentObj;
        }

        public object? Invoke()
        {
            return ActionParameters.Invoke(ParentObj);
        }

        public string GetHelpString() {
            var result = "";

            result += ActionParameters.GetHelpString();
            result += "\n";
            result += Options.GetOptionsString();

            return result;
        }
    }
}