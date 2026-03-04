using System;
using System.Collections.Generic;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.Parsing;

internal sealed class OptionReader(IEnumerator<string> argsEnumerator, IKeyValueStore<string, string?> store) : OptionReaderBase(argsEnumerator) {
    public IKeyValueStore<string, string?> Store { get; } = store;

    protected override Type GetOptionType(char option) => Store.GetValueType(option.ToString());

    protected override Type GetOptionType(string option) => Store.GetValueType(option);

    protected override bool HasOption(char option) => Store.ContainsKey(option.ToString());

    protected override bool HasOption(string option) => Store.ContainsKey(option);
}
