using System;
using System.Collections.Generic;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.Parsing;

internal class OptionReader : OptionReaderBase {
    public OptionReader(IEnumerator<string> argsEnumerator, IKeyValueStore<string, string?> store) :
        base(argsEnumerator) => Store = store;

    public IKeyValueStore<string, string?> Store { get; }

    protected override Type GetOptionType(char option) => Store.GetValueType(option.ToString());

    protected override Type GetOptionType(string option) => Store.GetValueType(option);

    protected override bool HasOption(char option) => Store.ContainsKey(option.ToString());

    protected override bool HasOption(string option) => Store.ContainsKey(option);
}
