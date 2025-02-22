using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using CalqFramework.Cli.InterfaceComponents;

namespace CalqFramework.Cli.Serialization {

    // TODO DRY
    public class HelpPrinter : IHelpPrinter {
        public object? PrintHelp(Type rootType, Submodule submodule, IEnumerable<Submodule> submodules, IEnumerable<Subcommand> subcommands, IEnumerable<Option> options) {
            string description = GetSummary(submodule.MemberInfo);
            if (!string.IsNullOrEmpty(description)) {
                Console.WriteLine(description);
                Console.WriteLine();
            }
            PrintHelp(submodules, subcommands, options);

            return null;
        }

        public object? PrintHelp(Type rootType, IEnumerable<Submodule> submodules, IEnumerable<Subcommand> subcommands, IEnumerable<Option> options) {
            var rootDescription = GetSummary(rootType);
            if (!string.IsNullOrEmpty(rootDescription)) {
                Console.WriteLine(rootDescription);
                Console.WriteLine();
            }
            PrintHelp(submodules, subcommands, options);

            return null;
        }

        public object? PrintSubcommandHelp(Type rootType, Subcommand subcommand, IEnumerable<Option> options) {
            PrintSubcommandDescription(subcommand);
            var sections = new SectionInfo[] {
                new() {
                    Title = "Parameters",
                    ItemInfos = subcommand.Parameters.OrderBy(y => y.ParameterInfo.Position).Select(x => new ItemInfo() { Description = GetDescription(x), Keys = x.Keys.Select(x => GetOptionKey(x)).ToList() }).ToList()
                },
                new() {
                    Title = "Options",
                    ItemInfos = options.Select(x => new ItemInfo() { Description = GetDescription(x), Keys = x.Keys.Select(x => GetOptionKey(x)).ToList() }).ToList()
                },
            };
            PrintSections(sections);

            return null;
        }

        private void PrintSubcommandDescription(Subcommand subcommand) {
            var parts = new List<string>();
            string summaryDescription = GetSummary(subcommand.MethodInfo);
            if (!string.IsNullOrEmpty(summaryDescription)) {
                parts.Add(summaryDescription);
            }
            string returnsDescription = GetReturn(subcommand.MethodInfo);
            if (!string.IsNullOrEmpty(returnsDescription)) {
                parts.Add(@returnsDescription);
            }
            string description = string.Join(Environment.NewLine, parts);
            if (!string.IsNullOrEmpty(description)) {
                Console.WriteLine(description);
                Console.WriteLine();
            }
        }

        private static string GetOptionKey(string key) {
            return key.Length > 1 ? $"--{key}" : $"-{key}";
        }

        private static string GetSummary(MemberInfo memberInfo) {
            string xmlFilePath = Path.ChangeExtension(memberInfo.Module.Assembly.Location, "xml");

            string memberName;
            Type? underlyingType = null;
            switch (memberInfo) {
                case MethodInfo method:
                    memberName = $"M:{method.DeclaringType?.FullName}.{method.Name}";
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length > 0) {
                        memberName += $"({string.Join(",", parameters.Select(p => p.ParameterType.FullName))})";
                    }
                    break;

                case PropertyInfo property:
                    memberName = $"P:{property.DeclaringType?.FullName}.{property.Name}";
                    underlyingType = property.PropertyType;
                    break;

                case FieldInfo field:
                    memberName = $"F:{field.DeclaringType?.FullName}.{field.Name}";
                    underlyingType = field.FieldType;
                    break;

                default:
                    throw new Exception("Unsupported member type.");
            }
            try {
                var doc = XDocument.Load(xmlFilePath);
                string? summary = doc.Descendants("member")
                    .FirstOrDefault(m => m.Attribute("name")?.Value == memberName)?
                    .Element("summary")?.Value;
                if (summary != null) {
                    summary = string.Join('\n', summary.Split('\n').Select(x => x.Trim()));
                }
                return summary?.Trim() ?? (underlyingType != null ? GetSummary(underlyingType) : "");
            } catch (FileNotFoundException) {
                throw new Exception($"Please add <GenerateDocumentationFile>true</GenerateDocumentationFile> to the csproj/fsproj file.");
            }
        }
        private static string GetSummary(ParameterInfo parameterInfo) {
            MemberInfo memberInfo = parameterInfo.Member;
            string xmlFilePath = Path.ChangeExtension(memberInfo.Module.Assembly.Location, "xml");

            string memberName;
            if (memberInfo is MethodBase method) {
                string methodName = method is ConstructorInfo ? "#ctor" : method.Name;
                memberName = $"M:{method.DeclaringType?.FullName}.{methodName}";
                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length > 0) {
                    memberName += $"({string.Join(",", parameters.Select(p => p.ParameterType.FullName))})";
                }
            } else if (memberInfo is PropertyInfo property) {
                memberName = $"P:{property.DeclaringType?.FullName}.{property.Name}";
            } else {
                throw new Exception("Unsupported member type for parameter.");
            }

            try {
                var doc = XDocument.Load(xmlFilePath);
                string? summary = doc.Descendants("member")
                    .FirstOrDefault(m => m.Attribute("name")?.Value == memberName)?
                    .Elements("param")
                    .FirstOrDefault(p => p.Attribute("name")?.Value == parameterInfo.Name)?
                    .Value;
                if (summary != null) {
                    summary = string.Join('\n', summary.Split('\n').Select(x => x.Trim()));
                }
                return summary?.Trim() ?? GetSummary(parameterInfo.ParameterType);
            } catch (FileNotFoundException) {
                throw new Exception("Please add <GenerateDocumentationFile>true</GenerateDocumentationFile> to the csproj/fsproj file.");
            }
        }

        private static string GetReturn(MemberInfo memberInfo) {
            string xmlFilePath = Path.ChangeExtension(memberInfo.Module.Assembly.Location, "xml");

            string memberName;
            if (memberInfo is MethodBase method) {
                string methodName = method is ConstructorInfo ? "#ctor" : method.Name;
                memberName = $"M:{method.DeclaringType?.FullName}.{methodName}";
                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length > 0) {
                    memberName += $"({string.Join(",", parameters.Select(p => p.ParameterType.FullName))})";
                }
            } else if (memberInfo is PropertyInfo property) {
                memberName = $"P:{property.DeclaringType?.FullName}.{property.Name}";
            } else {
                throw new Exception("Unsupported member type for parameter.");
            }

            try {
                var doc = XDocument.Load(xmlFilePath);
                string? summary = doc.Descendants("member")
                    .FirstOrDefault(m => m.Attribute("name")?.Value == memberName)?
                    .Element("returns")?
                    .Value;
                if (summary != null) {
                    summary = string.Join('\n', summary.Split('\n').Select(x => x.Trim()));
                }
                return summary?.Trim() ?? "";
            } catch (FileNotFoundException) {
                throw new Exception("Please add <GenerateDocumentationFile>true</GenerateDocumentationFile> to the csproj/fsproj file.");
            }
        }

        private static string GetSummary(Type type) {
            string xmlFilePath = Path.ChangeExtension(type.Assembly.Location, "xml");
            string memberName = $"T:{type.FullName}";

            try {
                var doc = XDocument.Load(xmlFilePath);
                string? summary = doc.Descendants("member")
                    .FirstOrDefault(m => m.Attribute("name")?.Value == memberName)?
                    .Element("summary")?.Value;
                if (summary != null) {
                    summary = string.Join('\n', summary.Split('\n').Select(x => x.Trim()));
                }
                return summary?.Trim() ?? "";
            } catch (FileNotFoundException) {
                // throw new Exception("Please add <GenerateDocumentationFile>true</GenerateDocumentationFile> to the csproj/fsproj file.");
            }

            return "";
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
        private class ItemInfo {
            public string Description { get; set; } = null!;
            public IList<string> Keys { get; set; } = null!;
        }

        private class SectionInfo {
            public IList<ItemInfo> ItemInfos { get; set; } = null!;
            public string Title { get; set; } = null!;
        }
    }
}
