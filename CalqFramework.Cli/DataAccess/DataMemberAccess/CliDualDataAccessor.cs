using CalqFramework.Cli.Serialization.Parsing;
using CalqFramework.Serialization.DataAccess;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.DataMemberAccess {
    internal class CliDualDataAccessor : DualDataAccessor<string, object?, MemberInfo> {

        public CliDualDataAccessor(IDataAccessor<string, object?, MemberInfo> primaryAccessor, IDataAccessor<string, object?, MemberInfo> secondaryAccessor) : base(primaryAccessor, secondaryAccessor) {
        }

        public override string ToString() {
            var result = "";

            var members = DataMediators.ToList();
            var options = members.Where(x => {
                return ValueParser.IsParseable(GetDataType(x));
            });
            var coreCommands = members.Where(x => {
                return !ValueParser.IsParseable(GetDataType(x));
            });

            result += "[CORE COMMANDS]\n";
            foreach (var command in coreCommands) {
                result += $"{ToStringHelper.MemberInfoToString(command)}\n";
            }

            result += "\n";
            result += "[OPTIONS]\n";
            foreach (var option in options) {
                var type = GetDataType(option);
                var defaultValue = this[option];
                result += $"{ToStringHelper.MemberInfoToString(option)} # {ToStringHelper.GetTypeName(type)} ({defaultValue})\n";
            }

            //Console.WriteLine();
            //Console.WriteLine("[ACTION COMMANDS]");
            //foreach (var methodInfo in methodResolver.Methods) {
            //    Console.WriteLine(methodResolver.MethodToString(methodInfo));
            //}

            return result;
        }
    }
}
