using System;
using System.Collections;

namespace CalqFramework.Options {
    public class OptionsDeserializer {
        public static int Deserialize<T>(T instance) where T : notnull {
            return Deserialize(instance, Environment.GetCommandLineArgs(), 1);
        }

        public static int Deserialize<T>(T instance, string[] args, int startIndex = 0) where T : notnull {
            var reader = new ToTypeOptionsReader<T>();
            Deserialize(reader, instance, args, startIndex);
            return reader.LastIndex;
        }

        private static void Deserialize<T>(ToTypeOptionsReader<T> reader, T instance, string[] args, int startIndex) where T : notnull {
            foreach (var (option, value) in reader.Read(args, startIndex)) {
                var type = Reflection.GetFieldOrPropertyType(typeof(T), option);
                var isCollection = type.GetInterface(nameof(ICollection)) != null;
                if (isCollection) {
                    type = type.GetGenericArguments()[0];
                }
                var valueObj = Reflection.ParseValue(type, value, option);
                try {
                    if (isCollection == false) {
                        Reflection.SetFieldOrPropertyValue(instance, option, valueObj);
                    } else {
                        var collection = Reflection.GetFieldOrPropertyValue(instance, option);
                        Reflection.AddChildValue((collection as ICollection)!, valueObj);
                    }
                } catch (ArgumentException ex) {
                    throw new Exception($"option and value type mismatch: {option}={value} ({option} is {type.Name})", ex);
                }
            }
        }

        public static void DeserializeSkipUnknown<T>(T instance) where T : notnull {
            DeserializeSkipUnknown(instance, Environment.GetCommandLineArgs(), 1);
        }

        public static void DeserializeSkipUnknown<T>(T instance, string[] args, int startIndex = 0) where T : notnull {
            var reader = new ToTypeOptionsReader<T>();
            while (reader.LastIndex < args.Length) {
                try {
                    Deserialize(reader, instance, args, startIndex);
                    if (reader.LastIndex == startIndex) {
                        ++startIndex;
                    } else {
                        if (args[reader.LastIndex - 1] == "--") {
                            break;
                        }
                        startIndex = reader.LastIndex;
                    }
                } catch (Exception ex) {
                    if (ex.Message.StartsWith("option doesn't exist")) {
                        startIndex = reader.LastIndex + 1;
                    } else {
                        throw;
                    }
                }
            }
        }
    }
}
