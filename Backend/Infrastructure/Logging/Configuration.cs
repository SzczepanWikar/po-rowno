using Microsoft.Extensions.Logging;

namespace Infrastructure.Logging
{
    public static class Configuration
    {
        public static ILoggingBuilder ConfigureLogging(this ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddConsole();

            return loggingBuilder;
        }
    }
}
