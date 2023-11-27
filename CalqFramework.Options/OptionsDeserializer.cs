using CalqFramework.Serialization.Text;
using System;
using System.Collections;

namespace CalqFramework.Options {
    public class OptionsDeserializer {
        public static int Deserialize<T>(T instance) where T : notnull {
            return Deserialize(instance, Environment.GetCommandLineArgs(), 1);
        }

        public static int Deserialize<T>(T instance, string[] args, int startIndex = 0) where T : notnull {
            var dataMemberAccessor = DataMemberAccess.DataMemberAccessorFactory.DefaultDataMemberAccessor;
            var reader = new ToTypeOptionsReader<T>(dataMemberAccessor);
            Deserialize(dataMemberAccessor, reader, instance, args, startIndex);
            return reader.LastIndex;
        }

        private static void Deserialize<T>(Serialization.DataMemberAccess.IDataMemberAccessor dataMemberAccessor, ToTypeOptionsReader<T> reader, T instance, string[] args, int startIndex) where T : notnull {
            foreach (var (option, value) in reader.Read(args, startIndex)) {
                //var type = Reflection.GetFieldOrPropertyType(typeof(T), option);
                var type = dataMemberAccessor.GetDataMemberType(typeof(T), option);
                var isCollection = type.GetInterface(nameof(ICollection)) != null;
                if (isCollection) {
                    type = type.GetGenericArguments()[0];
                }
                var valueObj = ValueParser.ParseValue(value, type, option);
                try {
                    if (isCollection == false) {
                        dataMemberAccessor.SetDataMemberValue(instance, option, valueObj);
                    } else {
                        var collection = dataMemberAccessor.GetDataMemberValue(instance, option);
                        CollectionMemberAccessor.AddChildValue((collection as ICollection)!, valueObj);
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
            var dataMemberAccessor = DataMemberAccess.DataMemberAccessorFactory.DefaultDataMemberAccessor;
            var reader = new ToTypeOptionsReader<T>(dataMemberAccessor);
            while (reader.LastIndex < args.Length) {
                try {
                    Deserialize(dataMemberAccessor, reader, instance, args, startIndex);
                    if (reader.LastIndex == startIndex) {
                        ++startIndex;
                    } else {
                        if (args[reader.LastIndex - 1] == "--") {
                            break;
                        }
                        startIndex = reader.LastIndex;
                    }
                } catch (MissingMemberException ex) {
                    startIndex = reader.LastIndex + 1;
                }
            }
        }
    }
}
