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
        services.AddSingleton<ILogWorker, LogWorker>();
        services.AddSingleton<IMailBuilderFactory, MailBuilderFactory>();
        services.AddSingleton<ForgotPasswordMailBuilder>();
        services.AddSingleton<IMailBuilder, ForgotPasswordMailBuilder>(item => item.GetService<ForgotPasswordMailBuilder>());
        services.AddSingleton<SetPasswordMailBuilder>();
        services.AddSingleton<IMailBuilder, SetPasswordMailBuilder>(item => item.GetService<SetPasswordMailBuilder>());
        services.AddSingleton<DeclineBookingMailBuilder>();
        services.AddSingleton<IMailBuilder, DeclineBookingMailBuilder>(item => item.GetService<DeclineBookingMailBuilder>());
        services.AddSingleton<ApprovalBookingMailBuilder>();
        services.AddSingleton<IMailBuilder, ApprovalBookingMailBuilder>(item => item.GetService<ApprovalBookingMailBuilder>());
        services.AddHostedService<Worker>();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("C://bphn/log-.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day)
            .CreateLogger();
    })
    .Build();

await host.RunAsync();
