using CalqFramework.Cli.DataAccess.ClassMember;
using CalqFramework.Cli.InterfaceComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    internal class OptionStore : IOptionStore {
        IClassMemberStore<string, string?, MemberInfo> Store { get; }

        public string? this[string key] { get => Store[key]; set => Store[key] = value; }

        public OptionStore(IClassMemberStore<string, string?, MemberInfo> store) {
            Store = store;
        }

        public bool ContainsKey(string key) {
            return Store.ContainsKey(key);
        }

        public Type GetDataType(string key) {
            return Store.GetDataType(key);
        }

        public string? GetValueOrInitialize(string key) {
            return Store.GetValueOrInitialize(key);
        }

        public IEnumerable<Option> GetOptions() {
            var result = new List<Option>();
            var dict = Store.GetKeysByAccessors();
            foreach (var key in dict.Keys) {
                result.Add(new Option() {
                    Type = GetDataType(dict[key].First()),
                    Keys = dict[key],
                    MemberInfo = key,
                    Value = this[dict[key].First()]
                });
            }
            return result;
        }
    }
}