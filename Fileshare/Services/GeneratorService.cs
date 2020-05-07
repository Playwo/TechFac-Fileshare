using System;
using System.Reflection.Emit;

namespace Fileshare.Services
{
    public class GeneratorService : Service
    {
        private readonly object IdGeneratorLock;
        private DateTimeOffset LastIdTime;

        public GeneratorService()
        {
            IdGeneratorLock = new object();
            LastIdTime = DateTimeOffset.UtcNow;
        }

        //ToDo: Add shorter filename
        public string GenerateNextName()
        {
            lock (IdGeneratorLock)
            {
                var time = DateTimeOffset.UtcNow;

                if ((time - LastIdTime).TotalMilliseconds < 2)
                {
                    time = LastIdTime.AddMilliseconds(2);
                }

                LastIdTime = time;

                return $"{time.Year}{time.Month}{time.Day}{time.Hour}{time.Minute}{time.Second}{time.Millisecond}";
            }
        }
    }
}
