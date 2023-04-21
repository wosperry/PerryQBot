using PerryQBot;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());
    builder.Host.UseAutofac();

    builder.Services.AddApplication<QBotModule>();
    var app = builder.Build();
    app.InitializeApplication();

    Log.Logger.Information("����ʼ����");
    app.Run();
}
catch (Exception ex)
{
    Log.Logger.Information(ex, "�������г���");
}