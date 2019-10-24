using FTPLogger;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using FTPLogger.BusinessLogic.StartupExtension;

[assembly: FunctionsStartup(typeof(Startup))]
namespace FTPLogger
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.RegisterAllServices();
        }
    }
}
