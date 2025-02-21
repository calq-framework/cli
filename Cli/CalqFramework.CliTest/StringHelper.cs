using System.Text.RegularExpressions;

namespace CalqFramework.CliTest {
    internal class StringHelper {
        public static string GetKebabCase(string value) {
            value = Regex.Replace(value, "([a-z0-9])([A-Z])", "$1-$2");
            value = Regex.Replace(value, "([a-zA-Z0-9])([A-Z][a-z])", "$1-$2");
            value = Regex.Replace(value, "[. ]", "-");
            value = value.ToLower();
            return value;
        }
    }
}
