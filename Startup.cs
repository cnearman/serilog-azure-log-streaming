using FunctionSerilog;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.AspNetCore;
using System;
using System.Collections.Generic;
using System.Text;
using Serilog.Formatting.Compact;

[assembly: FunctionsStartup(typeof(Startup))]

namespace FunctionSerilog
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var executionContextOptions = builder.Services.BuildServiceProvider()
                .GetService<IOptions<ExecutionContextOptions>>().Value;
            var currentDirectory = executionContextOptions.AppDirectory;

            Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                        .Enrich.FromLogContext()
                        .WriteTo.Console(new RenderedCompactJsonFormatter())
                        .WriteTo.File(
                            new RenderedCompactJsonFormatter(),
                            @"/LogFiles/Application/myapp.txt",
                            fileSizeLimitBytes: 1_000_000,
                            rollOnFileSizeLimit: true,
                            shared: true,
                            flushToDiskInterval: TimeSpan.FromSeconds(1))
                        .CreateLogger();

            builder.Services.AddLogging(lb => lb.AddSerilog());
        }
        public static string GetEnvironmentVariable(string name)

        {
            return name + ": " + System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
