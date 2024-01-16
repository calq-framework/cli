using CalqFramework.Serialization.DataAccess;
using System;

namespace CalqFramework.Cli.Serialization.Parsing
{
    internal class OptionsReader : OptionsReaderBase
    {
        private IDataAccessor DataAccessor { get; }

        public OptionsReader(string[] args, IDataAccessor dataAccessor) : base(args)
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

        protected override Type GetOptionType(string option)
        {
            return DataAccessor.GetType(option);
        }
    }
}
