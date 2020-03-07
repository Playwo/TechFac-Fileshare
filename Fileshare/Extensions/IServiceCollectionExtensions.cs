using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Fileshare.Extensions
{
    public static partial class Extension
    {
        public static void AddServices(this IServiceCollection serviceCollection)
        {
            var types = Assembly.GetEntryAssembly().GetServices();

            foreach (var type in types)
            {
                serviceCollection.AddSingleton(type);
            }
        }
    }
}
