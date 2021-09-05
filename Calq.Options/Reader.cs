using System;

namespace Calq.Options {
    public class Reader<T> : ReaderBase {
        protected override string GetOptionName(char option) {
            var type = typeof(T);
            foreach (var field in type.GetFields()) {
                if (field.Name[0] == option) {
                    return field.Name;
                }
            }
            foreach (var property in type.GetType().GetProperties()) {
                if (property.Name[0] == option) {
                    return property.Name;
                }
            }
            throw new Exception($"option doesn't exist: {option}");
        }

        protected override Type GetOptionType(string option) {
            var type = typeof(T);
            return Reflection.GetFieldOrPropertyType(type, option);
        }
    }
}
