using CalqFramework.Serialization.DataAccess;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.Serialization.Parsing
{
    internal class OptionsReader : OptionsReaderBase
    {
        private IDataAccessor<string, object?> DataAccessor { get; }

        public OptionsReader(IEnumerator<string> argsEnumerator, IDataAccessor<string, object?> dataAccessor) : base(argsEnumerator)
        {
            DataAccessor = dataAccessor;
        }

        protected override bool HasOption(char option)
        {
            return DataAccessor.ContainsKey(option.ToString());
        }

        protected override bool HasOption(string option)
        {
            return DataAccessor.ContainsKey(option);
        }

        protected override Type GetOptionType(char option) {
            return DataAccessor.GetDataType(option.ToString());
        }

        protected override Type GetOptionType(string option)
        {
            return DataAccessor.GetDataType(option);
        }
    }
}
