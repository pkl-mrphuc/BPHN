using BPHN.BusinessLayer.ImpServices;
using BPHN.BusinessLayer.ImpServices.MailBuilders;
using BPHN.BusinessLayer.IServices;
using BPHN.ImpRabbitMQLayer;
using BPHN.IRabbitMQLayer;
using BPHN.MailWorker;
using BPHN.ModelLayer;
using Serilog;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        services.AddSingleton<IRabbitMQProducerService, RabbitMQProducerService>();
        services.AddSingleton<IRabbitMQComsumerService, RabbitMQConsumerService>();
        services.AddSingleton<IKeyGenerator, KeyGenerator>();
        services.AddSingleton<IEmailWorker, EmailWorker>();
        services.AddSingleton<IMailBuilderFactory, MailBuilderFactory>();
        services.AddSingleton<ResetPasswordMailBuilder>();
        services.AddSingleton<IMailBuilder, ResetPasswordMailBuilder>(item => item.GetService<ResetPasswordMailBuilder>());
        services.AddHostedService<Worker>();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("C://bphn/log-.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day)
            .CreateLogger();
    })
    .Build();

await host.RunAsync();
