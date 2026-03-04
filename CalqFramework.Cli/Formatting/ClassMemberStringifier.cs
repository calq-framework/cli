using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CalqFramework.Cli.Formatting;

/// <summary>
///     Converts class member names to CLI-friendly kebab-case format.
/// </summary>
public class ClassMemberStringifier : ClassMemberStringifierBase {
    protected override IEnumerable<string> GetAlternativeNames(string name,
        IEnumerable<CliNameAttribute> cliNameAttributes) {
        List<string> keys = [];
        if (cliNameAttributes.Count() == 1) {
            string cliName = cliNameAttributes.First().Name;
            if (cliName.Length != 1) {
                keys.Add(cliName[0].ToString());
            }
        }

        if (!cliNameAttributes.Any()) {
            string kebabName = GetKebabCase(name);
            keys.Add(kebabName);
            if (kebabName.Length != 1) {
                keys.Add(kebabName[0].ToString());
            }
        }

        return keys;
    }

    protected override IEnumerable<string> GetRequiredNames(string name,
        IEnumerable<CliNameAttribute> cliNameAttributes) {
        List<string> keys = [];
        foreach (CliNameAttribute cliNameAttribute in cliNameAttributes) {
            keys.Add(cliNameAttribute.Name);
        }

        if (keys.Count == 0) {
            keys.Add(GetKebabCase(name));
        }

        return keys;
    }

    protected static string GetKebabCase(string value) {
        value = Regex.Replace(value, "([a-z0-9])([A-Z])", "$1-$2");
        value = Regex.Replace(value, "([a-zA-Z0-9])([A-Z][a-z])", "$1-$2");
        value = Regex.Replace(value, "[. ]", "-");
        value = value.ToLower();
        return value;
    }
}
