using System.Collections.Generic;

namespace CalqFramework.Cli.Completion {

    public interface ICompletionHandler {
        
        ResultVoid Handle(ICliContext context, string subcommand, IEnumerable<string> args, object target);
    }
}
