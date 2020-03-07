using System;
using System.Linq;
using System.Reflection;
using Fileshare.Services;

namespace Fileshare.Extensions
{
    public static partial class Extension
    {
        public static Type[] GetServices(this Assembly assembly)
            => assembly.GetTypes()
                       .Where(x => x.BaseType == typeof(Service))
                       .ToArray();
    }
}
