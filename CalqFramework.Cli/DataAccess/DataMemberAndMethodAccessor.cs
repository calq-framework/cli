﻿using CalqFramework.Cli.Serialization;
using CalqFramework.Serialization.DataAccess.DataMemberAccess;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CalqFramework.Cli.DataAccess
{
    internal class DataMemberAndMethodAccessor : IDataMemberAccessor
    {
        private IDataMemberAccessor DataMemberAccessor { get; }
        public object Obj { get; }
        public BindingFlags BindingAttr { get; }
        public IEnumerable<MethodInfo> Methods { get => Obj.GetType().GetMethods(BindingAttr).Where(x => !IsDotnetSpecific(x)); }

        public DataMemberAndMethodAccessor(IDataMemberAccessor dataMemberAccessor, BindingFlags methodBindingAttr)
        {
            DataMemberAccessor = dataMemberAccessor;
            Obj = dataMemberAccessor.Obj;
            BindingAttr = methodBindingAttr;
        }

        private static bool IsDotnetSpecific(MethodInfo methodInfo) {
            return methodInfo.DeclaringType == typeof(object) || methodInfo.GetCustomAttributes<CompilerGeneratedAttribute>().Any();
        }

        public MethodInfo GetMethod(string key) {
            var result = Obj.GetType().GetMethod(key, BindingAttr);
            if (result != null && !IsDotnetSpecific(result)) {
                return result;
            } else {
                throw new CliException($"invalid command");
            }
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

        public string DataMemberToString(MemberInfo memberInfo) {
            return DataMemberAccessor.DataMemberToString(memberInfo);
        }

        public bool HasDataMember(MemberInfo memberInfo) {
            return DataMemberAccessor.HasDataMember(memberInfo);
        }

        public bool Contains(MemberInfo member) {
            return DataMemberAccessor.Contains(member);
        }

        public Type GetType(MemberInfo member) {
            return DataMemberAccessor.GetType(member);
        }

        public object? GetValue(MemberInfo member) {
            return DataMemberAccessor.GetValue(member);
        }

        public object GetOrInitializeValue(MemberInfo member) {
            return DataMemberAccessor.GetOrInitializeValue(member);
        }

        public void SetValue(MemberInfo member, object? value) {
            DataMemberAccessor.SetValue(member, value);
        }

        public bool SetOrAddValue(MemberInfo member, object? value) {
            return DataMemberAccessor.SetOrAddValue(member, value);
        }
    }
}