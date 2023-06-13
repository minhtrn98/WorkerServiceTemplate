using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using WorkerServiceTemplate;
using Serilog;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Worker Service Template";
    })
    .ConfigureWebHostDefaults(builder =>
    {
        builder.Configure(app =>
        {
            app.UseRouting();

            app.UseHangfireDashboard();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHangfireDashboard();
            });
        });
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHangfire(conf => conf.UseSqlServerStorage("connection string"));
        services.AddHangfireServer();

        services.AddHostedService<Worker>();
    })
    .UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext())
    .Build();

await host.RunAsync();
