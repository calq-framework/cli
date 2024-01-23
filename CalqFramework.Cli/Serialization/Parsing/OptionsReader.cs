using CalqFramework.Serialization.DataAccess;
using System;
using System.Collections.Generic;

namespace CalqFramework.Cli.Serialization.Parsing
{
    internal class OptionsReader : OptionsReaderBase
    {
        private IDataAccessor DataAccessor { get; }

        public OptionsReader(IEnumerator<string> argsEnumerator, IDataAccessor dataAccessor) : base(argsEnumerator)
        {
            DataAccessor = dataAccessor;
        }

        protected override bool HasOption(char option)
        {
            return DataAccessor.HasKey(option.ToString());
        }

        protected override bool HasOption(string option)
        {
            return DataAccessor.HasKey(option);
        }

        protected override Type GetOptionType(char option) {
            return DataAccessor.GetType(option.ToString());
        }

        protected override Type GetOptionType(string option)
        {
            return DataAccessor.GetType(option);
        }
    }
}
