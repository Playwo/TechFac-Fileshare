using System;
using System.Reflection;
using System.Threading.Tasks;
using Fileshare.Services;

namespace Fileshare.Extensions
{
    public static partial class Extension
    {
        public static async Task RunServicesAsync(this IServiceProvider provider)
        {
            var types = Assembly.GetEntryAssembly().GetServices();

            foreach (var type in types)
            {
                var service = provider.GetService(type) as Service;
                await service.RunAsync();
            }
        }
    }
}
