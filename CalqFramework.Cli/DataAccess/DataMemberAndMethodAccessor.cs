using CalqFramework.Serialization.DataAccess.DataMemberAccess;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess
{
    internal class DataMemberAndMethodAccessor : IDataMemberAccessor
    {
        private IDataMemberAccessor DataMemberAccessor { get; }
        public object Obj { get; }
        public BindingFlags BindingAttr { get; }
        public MethodInfo[] Methods { get => Obj.GetType().GetMethods(BindingAttr); }

        public DataMemberAndMethodAccessor(IDataMemberAccessor dataMemberAccessor, BindingFlags methodBindingAttr)
        {
            DataMemberAccessor = dataMemberAccessor;
            Obj = dataMemberAccessor.Obj;
            BindingAttr = methodBindingAttr;
        }

        public bool TryGetDataMember(string key, out MemberInfo result)
        {
            return DataMemberAccessor.TryGetDataMember(key, out result);
        }

        public MemberInfo GetDataMember(string key)
        {
            return DataMemberAccessor.GetDataMember(key);
        }

        public IDictionary<string, MemberInfo> GetDataMembersByKeys()
        {
            return DataMemberAccessor.GetDataMembersByKeys();
        }

        public bool HasKey(string key)
        {
            return DataMemberAccessor.HasKey(key);
        }

        public Type GetType(string key)
        {
            return DataMemberAccessor.GetType(key);
        }

        public object? GetValue(string key)
        {
            return DataMemberAccessor.GetValue(key);
        }

        public object GetOrInitializeValue(string key)
        {
            return DataMemberAccessor.GetOrInitializeValue(key);
        }

        public void SetValue(string key, object? value)
        {
            DataMemberAccessor.SetValue(key, value);
        }

        public bool SetOrAddValue(string key, object? value)
        {
            return DataMemberAccessor.SetOrAddValue(key, value);
        }
    }
}