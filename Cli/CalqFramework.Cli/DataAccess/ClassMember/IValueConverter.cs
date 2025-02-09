﻿using System;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.ClassMember {
    public interface IValueConverter<TValue> {
        TValue ConvertFromInternalValue(object? value, Type internalType);

        object? ConvertToInternalValue(TValue value, Type internalType);
    }
}