using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tago.Extensions.Configuration;

namespace Tago.Extensions.Jwt.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureLogging((context, builder) =>
                    {
                        builder.AddFile(context.Configuration.GetSection("FileLoggingOptions"));
                    });

                    webBuilder.AddEncryptedJsonFile();

                    webBuilder.UseStartup<Startup>();
                });
    }
}
