using CalqFramework.Cli.DataAccess.DataMemberAccess;
using CalqFramework.Cli.Serialization.Parsing;
using System;
using static CalqFramework.Cli.Serialization.Parsing.OptionsReaderBase;

namespace CalqFramework.Cli.Serialization
{
    public class CliOptionsDeserializer
    {
        public static void Deserialize(object targetObj)
        {
            Deserialize(targetObj, new CliDeserializerOptions());
        }

        public static void Deserialize(object targetObj, CliDeserializerOptions options)
        {
            Deserialize(targetObj, options, Environment.GetCommandLineArgs(), 1);
        }

        public static void Deserialize(object targetObj, string[] args, int startIndex = 0)
        {
            Deserialize(targetObj, new CliDeserializerOptions(), args, startIndex);
        }

        public static void Deserialize(object targetObj, CliDeserializerOptions options, string[] args, int startIndex = 0)
        {
            var accessor = new DataMemberAccessorFactory(options.DataMemberAccessorOptions).CreateDataMemberAccessor(targetObj);
            var reader = new OptionsReader(args, accessor) { StartIndex = startIndex };

            foreach (var (option, value, optionAttr) in reader.Read())
            {
                if (optionAttr.HasFlag(OptionFlags.ValueUnassigned))
                {
                    if (optionAttr.HasFlag(OptionFlags.AmbigousValue))
                    {
                        throw new CliException($"ambiguous syntax around {option} (try using --)");
                    }
                    if (options.SkipUnknown)
                    {
                        continue;
                    }
                    else
                    {
                        throw new CliException($"unexpected value {option}");
                    }
                }

                var type = accessor.GetType(option);
                var valueObj = ValueParser.ParseValue(value, type, option);

                accessor.SetOrAddValue(option, valueObj);
            }
        }
    }
}
