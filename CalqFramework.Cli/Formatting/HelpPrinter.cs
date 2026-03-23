using CalqFramework.Cli.InterfaceComponents;

namespace CalqFramework.Cli.Formatting;

/// <summary>
///     Prints formatted help information for CLI components using XML documentation.
/// </summary>
public class HelpPrinter : IHelpPrinter {
    private readonly bool _supportsColor = DetectColorSupport();

    public void PrintHelp(ICliContext context, Type rootType, Submodule submodule, IEnumerable<Submodule> submodules, IEnumerable<Subcommand> subcommands, IEnumerable<Option> options) {
        string description = GetSummary(submodule.MemberInfo);
        if (!string.IsNullOrEmpty(description)) {
            context.InterfaceOut.WriteLine(description);
            context.InterfaceOut.WriteLine();
        }

        PrintHelp(context, submodules, subcommands, options);
    }

    public void PrintHelp(ICliContext context, Type rootType, IEnumerable<Submodule> submodules, IEnumerable<Subcommand> subcommands, IEnumerable<Option> options) {
        string rootDescription = GetSummary(rootType);
        if (!string.IsNullOrEmpty(rootDescription)) {
            context.InterfaceOut.WriteLine(rootDescription);
            context.InterfaceOut.WriteLine();
        }

        PrintHelp(context, submodules, subcommands, options);
    }

    public void PrintSubcommandHelp(ICliContext context, Type rootType, Subcommand subcommand, IEnumerable<Option> options) {
        PrintSubcommandDescription(context, subcommand);
        SectionInfo[] sections = [
            new() {
                Title = "Parameters",
                ItemInfos = [
                    .. subcommand.Parameters.Select(x => new ItemInfo {
                        Description = GetDescription(x),
                        Keys = [.. x.Keys.Select(x => GetOptionKey(x))]
                    })
                ]
            },
            new() {
                Title = "Options",
                ItemInfos = [
                    .. options.Select(x => new ItemInfo {
                        Description = GetDescription(x),
                        Keys = [.. x.Keys.Select(x => GetOptionKey(x))]
                    })
                ]
            }
        ];
        PrintSections(context, sections);
    }

    /// <summary>
    ///     Detects if the current terminal supports ANSI color codes.
    /// </summary>
    private static bool DetectColorSupport() {
        // If output is redirected (to file, pipe, etc.), disable colors
        if (Console.IsOutputRedirected) {
            return false;
        }

        try {
            // Check for NO_COLOR environment variable (universal standard)
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("NO_COLOR"))) {
                return false;
            }

            // Check for COLORTERM environment variable (indicates truecolor support)
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("COLORTERM"))) {
                return true;
            }

            // Check for TERM environment variable
            string? term = Environment.GetEnvironmentVariable("TERM");
            if (!string.IsNullOrEmpty(term)) {
                // Explicitly disable for dumb terminals
                if (term == "dumb") {
                    return false;
                }

                // Most modern terminals set TERM to something like "xterm-256color"
                if (term.Contains("color") || term.StartsWith("xterm") || term.StartsWith("screen") || term.StartsWith("tmux") || term == "linux" || term == "cygwin") {
                    return true;
                }
            }

            // Windows-specific checks
            if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                // Try to enable VT100 mode on Windows 10+
                Version version = Environment.OSVersion.Version;
                if (version.Major >= 10) {
                    return TryEnableWindowsVirtualTerminal();
                }
            }

            // Default to true for interactive terminals on Unix-like systems
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX) {
                return true;
            }
        } catch {
            // If any error occurs during detection, default to no color
            return false;
        }

        return false;
    }

    /// <summary>
    ///     Attempts to enable Virtual Terminal processing on Windows 10+.
    /// </summary>
    private static bool TryEnableWindowsVirtualTerminal() {
        try {
            IntPtr handle = GetStdHandle(-11); // STD_OUTPUT_HANDLE
            if (handle == IntPtr.Zero) {
                return false;
            }

            if (!GetConsoleMode(handle, out uint mode)) {
                return false;
            }

            // ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004
            mode |= 0x0004;

            return SetConsoleMode(handle, mode);
        } catch {
            return false;
        }
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    private static string GetMemberXMLName(MemberInfo memberInfo) {
        switch (memberInfo) {
            case MethodBase methodBase:
                string methodName = methodBase is ConstructorInfo ? "#ctor" : methodBase.Name;
                string declaringType = methodBase.DeclaringType?.FullName!;
                ParameterInfo[] parameters = methodBase.GetParameters();
                string paramsPart = parameters.Length > 0 ? $"({string.Join(",", parameters.Select(p => p.ParameterType.FullName))})" : "";
                return $"M:{declaringType}.{methodName}{paramsPart}";
            case PropertyInfo prop:
                return $"P:{prop.DeclaringType?.FullName}.{prop.Name}";
            case FieldInfo field:
                return $"F:{field.DeclaringType?.FullName}.{field.Name}";
            default:
                throw CliErrors.UnsupportedMemberType(
                    memberInfo.GetType()
                        .Name);
        }
    }

    private static string GetOptionKey(string key) => key.Length > 1 ? $"--{key}" : $"-{key}";

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
        return !string.IsNullOrEmpty(summary) ? summary : underlyingType != null ? GetSummary(underlyingType) : "";
    }

    private static string GetSummary(ParameterInfo parameterInfo) {
        MemberInfo memberInfo = parameterInfo.Member;
        string xmlFilePath = GetXmlFilePath(memberInfo.Module.Assembly);
        string memberName = GetMemberXMLName(memberInfo);

        string paramSummary = GetXmlDocumentation(xmlFilePath, memberName, "param", parameterInfo.Name);
        return !string.IsNullOrEmpty(paramSummary) ? paramSummary : GetSummary(parameterInfo.ParameterType);
    }

    private static string GetSummary(Type type) {
        string xmlFilePath = GetXmlFilePath(type.Assembly);
        string memberName = GetTypeMemberName(type);
        return GetXmlDocumentation(xmlFilePath, memberName, "summary");
    }

    private static string GetTypeMemberName(Type type) => $"T:{type.FullName}";

    private static string GetXmlDocumentation(string xmlFilePath, string memberName, string elementName, string? paramName = null) {
        if (!File.Exists(xmlFilePath)) {
            return string.Empty;
        }

        XDocument doc = XDocument.Load(xmlFilePath);
        XElement? memberElement = doc.Descendants("member")
            .FirstOrDefault(m => m.Attribute("name")
                ?.Value == memberName);
        if (memberElement == null) {
            return "";
        }

        XElement? targetElement = null;
        if (paramName != null) {
            targetElement = memberElement.Elements("param")
                .FirstOrDefault(p => p.Attribute("name")
                    ?.Value == paramName);
        } else {
            targetElement = memberElement.Element(elementName);
        }

        if (targetElement == null) {
            return "";
        }

        string value = targetElement.Value;
        value = string.Join(
            Environment.NewLine,
            value.Split(Environment.NewLine)
                .Select(x => x.Trim()));
        return value;
    }

    private static string GetXmlFilePath(Assembly assembly) => Path.ChangeExtension(assembly.Location, "xml");

    private void SetConsoleColor(int red, int green, int blue) {
        if (_supportsColor) {
            Console.Write($"\u001b[38;2;{red};{green};{blue}m");
        }
    }

    private void ResetConsoleColor() {
        if (_supportsColor) {
            Console.Write("\u001b[0m");
        }
    }

    private static string GetDescription(Submodule item) => GetSummary(item.MemberInfo);

    private static string GetDescription(Subcommand item) => GetSummary(item.MethodInfo);

    private static string GetDescription(Parameter item) => GetDescription(item.ValueType, item.IsMultiValue, item.Value, item.HasDefaultValue, GetSummary(item.ParameterInfo));

    private static string GetDescription(Option item) => GetDescription(item.ValueType, item.IsMultiValue, item.Value, true, GetSummary(item.MemberInfo));

    private static string GetDescription(Type type, bool isMultiValue, string? defaultValue, bool hasDefaultValue, string? summary) {
        List<string> parts = [];

        if (!hasDefaultValue) {
            string typeDescription = isMultiValue ? "list of " + GetTypeDescription(type) : GetTypeDescription(type);
            parts.Add($"(Requires: {typeDescription})");
        }

        if (hasDefaultValue) {
            defaultValue = type == typeof(string) && defaultValue != null ? $"'{defaultValue}'" : defaultValue;
            defaultValue ??= "NULL";
            parts.Add($"(Default: {defaultValue})");
        }

        if (!string.IsNullOrWhiteSpace(summary)) {
            parts.Add("-- " + summary.Trim());
        }

        return string.Join(" ", parts);
    }

    private static List<int> GetMaxKeyLengths(IEnumerable<SectionInfo> sections) {
        List<int> maxLengths = [];
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

    private static string GetTypeDescription(Type type) {
        if (type.IsPrimitive) {
            return type.Name.ToLowerInvariant();
        }

        if (type.IsEnum) {
            string values = string.Join(", ", Enum.GetNames(type));
            return $"{values}";
        }

        return type.Name;
    }

    private static List<int> NormalizeKeyCounts(IEnumerable<SectionInfo> sections) {
        List<int> maxLengths = GetMaxKeyLengths(sections);

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

    private void PrintHelp(ICliContext context, IEnumerable<Submodule> submodules, IEnumerable<Subcommand> subcommands, IEnumerable<Option> options) {
        SectionInfo[] sections = [
            new() {
                Title = "Submodules",
                ItemInfos = [
                    .. submodules.Select(x => new ItemInfo {
                        Description = GetDescription(x),
                        Keys = [.. x.Keys]
                    })
                ]
            },
            new() {
                Title = "Subcommands",
                ItemInfos = [
                    .. subcommands.Select(x => new ItemInfo {
                        Description = GetDescription(x),
                        Keys = [.. x.Keys]
                    })
                ]
            },
            new() {
                Title = "Options",
                ItemInfos = [
                    .. options.Select(x => new ItemInfo {
                        Description = GetDescription(x),
                        Keys = [.. x.Keys.Select(x => GetOptionKey(x))]
                    })
                ]
            }
        ];
        PrintSections(context, sections);
    }

    private void PrintSections(ICliContext context, IEnumerable<SectionInfo> sections) {
        List<int> maxLengths = NormalizeKeyCounts(sections);

        SectionInfo? firstSection = sections.SkipWhile(x => x.ItemInfos.Count == 0)
            .FirstOrDefault();
        if (firstSection == null) return;
        foreach (SectionInfo section in sections) {
            if (section.ItemInfos.Count == 0) {
                continue;
            }

            if (section != firstSection) {
                context.InterfaceOut.WriteLine();
            }

            SetConsoleColor(90, 147, 241);
            context.InterfaceOut.WriteLine(section.Title);
            foreach (ItemInfo item in section.ItemInfos) {
                context.InterfaceOut.Write("  "); // ident
                IList<string> keys = item.Keys;
                string[] parts = new string[maxLengths.Count];
                for (int i = 0; i < maxLengths.Count; i++) {
                    if (i == 0) {
                        parts[i] = keys[i]
                            .PadRight(maxLengths[i]);
                    } else {
                        parts[i] = keys[i]
                            .PadLeft(maxLengths[i]);
                    }
                }

                SetConsoleColor(149, 184, 204);
                context.InterfaceOut.Write(string.Join(" ", parts));
                ResetConsoleColor();
                context.InterfaceOut.Write("  "); // keys and rootDescription space
                context.InterfaceOut.WriteLine(item.Description);
            }
        }

        ResetConsoleColor();
    }

    private static void PrintSubcommandDescription(ICliContext context, Subcommand subcommand) {
        List<string> parts = [];
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
            context.InterfaceOut.WriteLine(description);
            context.InterfaceOut.WriteLine();
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
