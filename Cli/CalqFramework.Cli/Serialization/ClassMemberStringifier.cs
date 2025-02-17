using System.Collections.Generic;
using System.Linq;

namespace CalqFramework.Cli.Serialization {

    public class ClassMemberStringifier : ClassMemberStringifierBase {
        protected override IEnumerable<string> GetAlternativeNames(string name, IEnumerable<CliNameAttribute> cliNameAttributes) {
            var keys = new List<string>();
            if (cliNameAttributes.Count() == 1) {
                var cliName = cliNameAttributes.First().Name;
                if (cliName.Length != 1) {
                    keys.Add(cliName[0].ToString());
                }
            }
            if (!cliNameAttributes.Any()) {
                keys.Add(name);
                if (name.Length != 1) {
                    keys.Add(name[0].ToString());
                }
            }
            return keys;
        }

        protected override IEnumerable<string> GetRequiredNames(string name, IEnumerable<CliNameAttribute> cliNameAttributes) {
            var keys = new List<string>();
            foreach (var cliNameAttribute in cliNameAttributes) {
                keys.Add(cliNameAttribute.Name);
            }
            if (!keys.Any()) {
                keys.Add(name);
            }
            return keys;
        }
    }
}
