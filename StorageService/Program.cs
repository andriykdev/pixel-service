using StorageService;
using StorageService.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<MessageConsumerWorker>();
builder.Services.AddSingleton<ILogWriter, FileLogWriter>();
builder.Services.AddSingleton<MessageHandler>();

var host = builder.Build();
host.Run();
