using GrabAndGo.Notification.Worker;
using GrabAndGo.BuildingBlocks.Observability;

var builder = Host.CreateApplicationBuilder(args);

builder.ConfigureLogging("notification-worker");
builder.Services.AddObservability(builder.Configuration, "notification-worker");

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
