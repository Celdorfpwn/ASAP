using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace GitJiraConfiguration
{
    static class Extensions
    {
        internal static IEnumerable<PropertyInfo> GetSettingProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(property => property.PropertyType.Equals(typeof(Setting)));
        }
    }
}
