using Ghbvft6.Calq.Options.Attributes;
using System;

namespace Ghbvft6.Calq.Options {
    public class Reader<T> : ReaderBase {
        protected override string GetOptionName(char option) {
            var type = typeof(T);

            string? firstLetterMatch = null;
            foreach (System.Reflection.MemberInfo member in type.GetFields()) {
                if (Attribute.GetCustomAttribute(member, typeof(ShortNameAttribute)) is ShortNameAttribute shortName && shortName.Name == option) {
                    return (Attribute.GetCustomAttribute(member, typeof(NameAttribute)) as NameAttribute)?.Name ?? member.Name;
                }
                var name = (Attribute.GetCustomAttribute(member, typeof(NameAttribute)) as NameAttribute)?.Name;
                if (name != null && name[0] == option) {
                    firstLetterMatch = name;
                } else if (member.Name[0] == option) {
                    firstLetterMatch = member.Name;
                }
            }
            foreach (System.Reflection.MemberInfo member in type.GetType().GetProperties()) {
                if (Attribute.GetCustomAttribute(member, typeof(ShortNameAttribute)) is ShortNameAttribute shortName && shortName.Name == option) {
                    return (Attribute.GetCustomAttribute(member, typeof(NameAttribute)) as NameAttribute)?.Name ?? member.Name;
                }
                var name = (Attribute.GetCustomAttribute(member, typeof(NameAttribute)) as NameAttribute)?.Name;
                if (name != null && name[0] == option) {
                    firstLetterMatch = name;
                } else if (member.Name[0] == option) {
                    firstLetterMatch = member.Name;
                }
            }

            return firstLetterMatch ?? throw new Exception($"option doesn't exist: {option}");
        }

        protected override Type GetOptionType(string option) {
            var type = typeof(T);
            return Reflection.GetFieldOrPropertyType(type, option);
        }
    }
}
