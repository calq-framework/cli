using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using CalqFramework.Cli.InterfaceComponents;

namespace CalqFramework.Cli.Formatting {

    /// <summary>
    /// Prints formatted help information for CLI components using XML documentation.
    /// </summary>
    public class HelpPrinter : IHelpPrinter {
        public void PrintHelp(Type rootType, Submodule submodule, IEnumerable<Submodule> submodules, IEnumerable<Subcommand> subcommands, IEnumerable<Option> options) {
            string description = GetSummary(submodule.MemberInfo);
            if (!string.IsNullOrEmpty(description)) {
                Console.WriteLine(description);
                Console.WriteLine();
            }
            PrintHelp(submodules, subcommands, options);
        }

        public void PrintHelp(Type rootType, IEnumerable<Submodule> submodules, IEnumerable<Subcommand> subcommands, IEnumerable<Option> options) {
            var rootDescription = GetSummary(rootType);
            if (!string.IsNullOrEmpty(rootDescription)) {
                Console.WriteLine(rootDescription);
                Console.WriteLine();
            }
            PrintHelp(submodules, subcommands, options);
        }

        public void PrintSubcommandHelp(Type rootType, Subcommand subcommand, IEnumerable<Option> options) {
            PrintSubcommandDescription(subcommand);
            var sections = new SectionInfo[] {
                new() {
                    Title = "Parameters",
                    ItemInfos = subcommand.Parameters.Select(x => new ItemInfo() { Description = GetDescription(x), Keys = x.Keys.Select(x => GetOptionKey(x)).ToList() }).ToList()
                },
                new() {
                    Title = "Options",
                    ItemInfos = options.Select(x => new ItemInfo() { Description = GetDescription(x), Keys = x.Keys.Select(x => GetOptionKey(x)).ToList() }).ToList()
                },
            };
            PrintSections(sections);
        }

        private static string GetMemberXMLName(MemberInfo memberInfo) {
            switch (memberInfo) {
                case MethodBase methodBase:
                    string methodName = methodBase is ConstructorInfo ? "#ctor" : methodBase.Name;
                    string declaringType = methodBase.DeclaringType?.FullName!;
                    var parameters = methodBase.GetParameters();
                    string paramsPart = parameters.Length > 0
                        ? $"({string.Join(",", parameters.Select(p => p.ParameterType.FullName))})"
                        : "";
                    return $"M:{declaringType}.{methodName}{paramsPart}";
                case PropertyInfo prop:
                    return $"P:{prop.DeclaringType?.FullName}.{prop.Name}";
                case FieldInfo field:
                    return $"F:{field.DeclaringType?.FullName}.{field.Name}";
                default:
                    throw CliErrors.UnsupportedMemberType(memberInfo.GetType().Name);
            }
        }

        private static string GetOptionKey(string key) {
            return key.Length > 1 ? $"--{key}" : $"-{key}";
        }

        private static string GetReturn(MemberInfo memberInfo) {
            string xmlFilePath = GetXmlFilePath(memberInfo.Module.Assembly);
            string memberName = GetMemberXMLName(memberInfo);
            return GetXmlDocumentation(xmlFilePath, memberName, "returns");
        }

        private static string GetSummary(MemberInfo memberInfo) {
            string xmlFilePath = GetXmlFilePath(memberInfo.Module.Assembly);
            string memberName = GetMemberXMLName(memberInfo);
            Type? underlyingType = memberInfo switch {
                PropertyInfo prop => prop.PropertyType,
                FieldInfo field => field.FieldType,
                _ => null
            };

            string summary = GetXmlDocumentation(xmlFilePath, memberName, "summary");
            return !string.IsNullOrEmpty(summary)
                ? summary
                : (underlyingType != null ? GetSummary(underlyingType) : "");
        }

        private static string GetSummary(ParameterInfo parameterInfo) {
            MemberInfo memberInfo = parameterInfo.Member;
            string xmlFilePath = GetXmlFilePath(memberInfo.Module.Assembly);
            string memberName = GetMemberXMLName(memberInfo);

            string paramSummary = GetXmlDocumentation(xmlFilePath, memberName, "param", parameterInfo.Name);
            return !string.IsNullOrEmpty(paramSummary)
                ? paramSummary
                : GetSummary(parameterInfo.ParameterType);
        }

        private static string GetSummary(Type type) {
            string xmlFilePath = GetXmlFilePath(type.Assembly);
            string memberName = GetTypeMemberName(type);
            return GetXmlDocumentation(xmlFilePath, memberName, "summary");
        }

        private static string GetTypeMemberName(Type type) {
            return $"T:{type.FullName}";
        }

        private static string GetXmlDocumentation(string xmlFilePath, string memberName, string elementName, string? paramName = null) {
            if (!File.Exists(xmlFilePath)) {
                return string.Empty;
            }

            var doc = XDocument.Load(xmlFilePath);
            var memberElement = doc.Descendants("member")
                .FirstOrDefault(m => m.Attribute("name")?.Value == memberName);
            if (memberElement == null) return "";

            XElement? targetElement = null;
            if (paramName != null) {
                targetElement = memberElement.Elements("param")
                    .FirstOrDefault(p => p.Attribute("name")?.Value == paramName);
            } else {
                targetElement = memberElement.Element(elementName);
            }

            if (targetElement == null) return "";

            string value = targetElement.Value;
            value = string.Join(Environment.NewLine, value.Split(Environment.NewLine).Select(x => x.Trim()));
            return value;
        }

        private static string GetXmlFilePath(Assembly assembly) {
            return Path.ChangeExtension(assembly.Location, "xml");
        }

        private static void SetConsoleColor(int red, int green, int blue) {
            Console.Write($"\u001b[38;2;{red};{green};{blue}m");
        }

        private string GetDescription(Submodule item) {
            return GetSummary(item.MemberInfo);
        }

        private string GetDescription(Subcommand item) {
            return GetSummary(item.MethodInfo);
        }

        private string GetDescription(Parameter item) {
            return GetDescription(item.Type, item.Value, item.HasDefaultValue, GetSummary(item.ParameterInfo));
        }

        private string GetDescription(Option item) {
            return GetDescription(item.Type, item.Value, true, GetSummary(item.MemberInfo));
        }

        private string GetDescription(Type type, string? defaultValue, bool hasDefaultValue, string? summary) {
            var parts = new List<string>();

            if (!hasDefaultValue) {
                string typeDescription = GetTypeDescription(type);
                parts.Add($"(Requires: {typeDescription})");
            }

            if (hasDefaultValue) {
                defaultValue = type == typeof(string) && defaultValue != null ? $"'{defaultValue}'" : defaultValue;
                defaultValue = defaultValue == null ? "NULL" : defaultValue;
                parts.Add($"(Default: {defaultValue})");
            }

            if (!string.IsNullOrWhiteSpace(summary)) {
                parts.Add("-- " + summary.Trim());
            }

            return string.Join(" ", parts);
        }

        private IList<int> GetMaxKeyLengths(IEnumerable<SectionInfo> sections) {
            var maxLengths = new List<int>();
            foreach (SectionInfo section in sections) {
                foreach (ItemInfo item in section.ItemInfos) {
                    IList<string> keys = item.Keys;
                    for (int i = 0; i < keys.Count; i++) {
                        int currentLength = keys[i].Length;
                        if (maxLengths.Count <= i) {
                            maxLengths.Add(0);
                        }
                        if (currentLength > maxLengths[i]) {
                            maxLengths[i] = currentLength;
                        }
                    }
                }
            }
            return maxLengths;
        }

        private string GetTypeDescription(Type type) {
            if (type.IsGenericType &&
                (typeof(IList<>).IsAssignableFrom(type.GetGenericTypeDefinition()) ||
                 type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>)))) {
                Type elementType = type.GetGenericArguments()[0];
                return "list of " + GetTypeDescription(elementType);
            }

            if (type.IsPrimitive) {
                return type.Name.ToLowerInvariant();
            }

            if (type.IsEnum) {
                string values = string.Join(", ", Enum.GetNames(type));
                return $"{values}";
            }

            return type.Name;
        }

        private IList<int> NormalizeKeyCounts(IEnumerable<SectionInfo> sections) {
            IList<int> maxLengths = GetMaxKeyLengths(sections);

            foreach (SectionInfo section in sections) {
                foreach (ItemInfo item in section.ItemInfos) {
                    IList<string> keys = item.Keys;
                    if (keys.Count < maxLengths.Count) {
                        int keyCountDifference = maxLengths.Count - keys.Count;
                        for (int i = 0; i < keyCountDifference; ++i) {
                            keys.Insert(1, string.Empty);
                        }
                    }
                }
            }
            return GetMaxKeyLengths(sections);
        }

        private void PrintHelp(IEnumerable<Submodule> submodules, IEnumerable<Subcommand> subcommands, IEnumerable<Option> options) {
            var sections = new SectionInfo[] {
                new() {
                    Title = "Submodules",
                    ItemInfos = submodules.Select(x => new ItemInfo() { Description = GetDescription(x), Keys = x.Keys.ToList() }).ToList()
                },
                new() {
                    Title = "Subcommands",
                    ItemInfos = subcommands.Select(x => new ItemInfo() { Description = GetDescription(x), Keys = x.Keys.ToList() }).ToList()
                },
                new() {
                    Title = "Options",
                    ItemInfos = options.Select(x => new ItemInfo() { Description = GetDescription(x), Keys = x.Keys.Select(x => GetOptionKey(x)).ToList() }).ToList()
                },
            };
            PrintSections(sections);
        }

        private void PrintSections(IEnumerable<SectionInfo> sections) {
            IList<int> maxLengths = NormalizeKeyCounts(sections);

            SectionInfo firstSection = sections.SkipWhile(x => x.ItemInfos.Count == 0).First();
            foreach (SectionInfo section in sections) {
                if (section.ItemInfos.Count == 0) {
                    continue;
                }
                if (section != firstSection) {
                    Console.WriteLine();
                }
                SetConsoleColor(80, 140, 240);
                Console.WriteLine(section.Title);
                foreach (ItemInfo item in section.ItemInfos) {
                    Console.Write("  "); // ident
                    IList<string> keys = item.Keys;
                    string[] parts = new string[maxLengths.Count];
                    for (int i = 0; i < maxLengths.Count; i++) {
                        if (i == 0) {
                            parts[i] = keys[i].PadRight(maxLengths[i]);
                        } else {
                            parts[i] = keys[i].PadLeft(maxLengths[i]);
                        }
                    }
                    SetConsoleColor(160, 200, 210);
                    Console.Write(string.Join(" ", parts));
                    Console.ResetColor();
                    Console.Write("  "); // keys and rootDescription space
                    Console.WriteLine(item.Description);
                }
            }
            Console.ResetColor();
        }

        private void PrintSubcommandDescription(Subcommand subcommand) {
            var parts = new List<string>();
            string summaryDescription = GetSummary(subcommand.MethodInfo);
            if (!string.IsNullOrEmpty(summaryDescription)) {
                parts.Add(summaryDescription);
            }
            string returnsDescription = GetReturn(subcommand.MethodInfo);
            if (!string.IsNullOrEmpty(returnsDescription)) {
                parts.Add("Returns -> " + returnsDescription);
            }
            string description = string.Join(Environment.NewLine, parts);
            if (!string.IsNullOrEmpty(description)) {
                Console.WriteLine(description);
                Console.WriteLine();
            }
        }
        private class ItemInfo {
            public required string Description { get; set; }
            public required IList<string> Keys { get; set; }
        }

        private class SectionInfo {
            public required IList<ItemInfo> ItemInfos { get; set; }
            public required string Title { get; set; }
        }
    }
}
