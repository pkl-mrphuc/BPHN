using BPHN.BusinessLayer.ImpServices;
using BPHN.BusinessLayer.ImpServices.MailBuilders;
using BPHN.BusinessLayer.IServices;
using BPHN.ImpRabbitMQLayer;
using BPHN.IRabbitMQLayer;
using BPHN.MailWorker;
using BPHN.ModelLayer;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        services.AddSingleton<IRabbitMQProducerService, RabbitMQProducerService>();
        services.AddSingleton<IRabbitMQComsumerService, RabbitMQConsumerService>();
        services.AddSingleton<IEmailWorker, EmailWorker>();
        services.AddSingleton<IMailBuilderFactory, MailBuilderFactory>();
        services.AddSingleton<ResetPasswordMailBuilder>();
        services.AddSingleton<IMailBuilder, ResetPasswordMailBuilder>(item => item.GetService<ResetPasswordMailBuilder>());
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
