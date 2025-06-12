using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacqlEngine.Helpers {
    using System.Reflection;

    public static class GenericFlattener {
        public static Dictionary<string, object> FlattenFull(object root) {
            var row = new Dictionary<string, object>();
            FlattenObject(root, row, prefix: "Game");
            return row;
        }

        private static void FlattenObject(object obj, Dictionary<string, object> data, string prefix) {
            if (obj == null)
                return;

            var type = obj.GetType();

            var teamName = obj.GetType().GetProperty("TeamName")?.GetValue(obj);
            var playerName = obj.GetType().GetProperty("PlayerName")?.GetValue(obj);

            foreach (var prop in type.GetProperties()) {
                var value = prop.GetValue(obj);
                var name = $"{prefix}.{prop.Name}";

                if (IsScalar(prop.PropertyType)) {
                    if (teamName != null && playerName != null) {
                        data[teamName+"_"+playerName + "_"+name] = value;
                    } else {
                        data[name] = value;
                    }
                } else if (IsList(prop.PropertyType)) {

                    var list = value as System.Collections.IEnumerable;
                    if (list == null)
                        continue;

                    int index = 0;
                    foreach (var item in list) {
                        var indexedPrefix = $"{name}[{index}]";
                        FlattenObject(item, data, indexedPrefix);
                        index++;
                    }
                } else if (prop.PropertyType.IsClass) {
                    FlattenObject(value, data, name);
                }
            }
        }

        private static bool IsScalar(Type t) {
            return t.IsPrimitive || t == typeof(string) || t == typeof(DateTime) || t == typeof(decimal) || t.IsEnum || Nullable.GetUnderlyingType(t) != null;
        }

        private static bool IsList(Type type) {
            return typeof(System.Collections.IEnumerable).IsAssignableFrom(type)
                   && type != typeof(string);
        }
    }

}
