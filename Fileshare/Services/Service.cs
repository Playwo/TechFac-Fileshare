using System.Threading.Tasks;

namespace Fileshare.Services
{
    public abstract class Service
    {
        public virtual ValueTask RunAsync()
            => new ValueTask();
    }
}
