using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;

namespace HttpEndpoint
{
    class Program
    {
        static void Main(string[] args)
        {
		AppContext.SetSwitch("System.Net.Http.UseManagedHttpClientHandler", true);

            var config = new ConfigurationBuilder()
                .Build();

            var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseKestrel(options => {
                }).UseUrls("http://+:5000")
                .UseSockets()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
