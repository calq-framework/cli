using System;

namespace Ghbvft6.Calq.Options {
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
                var valueObj = Reflection.ParseValue(Reflection.GetFieldOrPropertyType(instance.GetType(), option), value, option);
                try {
                    Reflection.SetFieldOrPropertyValue(instance, option, valueObj);
                } catch (ArgumentException ex) {
                    var type = Reflection.GetFieldOrPropertyType(typeof(T), option);
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
