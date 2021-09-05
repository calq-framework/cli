using System;

namespace Calq.Options {
    public class Opts {
        public static void Load<T>(string[] args, T instance) {
            foreach (var (option, value) in new Reader<T>().Read(args)) {
                var valueObj = Reflection.ParseValue(instance, option, value);
                try {
                    Reflection.SetFieldOrPropertyValue(instance, option, valueObj);
                } catch (ArgumentException ex) {
                    var type = Reflection.GetFieldOrPropertyType(typeof(T), option);
                    throw new Exception($"option and value type mismatch: {option}={value} ({option} is {type.Name})", ex);
                }
            }
        }
    }
}
