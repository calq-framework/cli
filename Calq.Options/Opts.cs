using System;

namespace Ghbvft6.Calq.Options {
    public class Opts {
        public static int Load<T>(T instance) where T : notnull {
            return Load(instance, Environment.GetCommandLineArgs(), 1);
        }

        public static int Load<T>(T instance, string[] args, int startIndex = 0) where T : notnull {
            var reader = new Reader<T>();
            foreach (var (option, value) in reader.Read(args, startIndex)) {
                var valueObj = Reflection.ParseValue(Reflection.GetFieldOrPropertyType(instance.GetType(), option), value, option);
                try {
                    Reflection.SetFieldOrPropertyValue(instance, option, valueObj);
                } catch (ArgumentException ex) {
                    var type = Reflection.GetFieldOrPropertyType(typeof(T), option);
                    throw new Exception($"option and value type mismatch: {option}={value} ({option} is {type.Name})", ex);
                }
            }
            return reader.LastIndex;
        }
    }
}
