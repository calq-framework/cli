using System;
using System.Collections;

namespace Ghbvft6.CalqFramework.Options {
    public class Opts {
        public static int Load<T>(T instance) where T : notnull {
            return Load(instance, Environment.GetCommandLineArgs(), 1);
        }

        public static int Load<T>(T instance, string[] args, int startIndex = 0) where T : notnull {
            var reader = new Reader<T>();
            Load(reader, instance, args, startIndex);
            return reader.LastIndex;
        }

        private static void Load<T>(Reader<T> reader, T instance, string[] args, int startIndex) where T : notnull {
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

        public static void LoadSkipUnknown<T>(T instance) where T : notnull {
            LoadSkipUnknown(instance, Environment.GetCommandLineArgs(), 1);
        }

        public static void LoadSkipUnknown<T>(T instance, string[] args, int startIndex = 0) where T : notnull {
            var reader = new Reader<T>();
            while (reader.LastIndex < args.Length) {
                try {
                    Load(reader, instance, args, startIndex);
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
