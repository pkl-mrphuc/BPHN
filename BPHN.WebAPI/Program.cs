using BPHN.BusinessLayer.ImpServices;
using BPHN.BusinessLayer.ImpServices.MailBuilders;
using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.ImpRepositories;
using BPHN.DataLayer.IRepositories;
using BPHN.ImpRabbitMQLayer;
using BPHN.IRabbitMQLayer;
using BPHN.ModelLayer;
using BPHN.WebAPI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IPitchService, PitchService>();
builder.Services.AddScoped<IPitchRepository, PitchRepository>();
builder.Services.AddScoped<IHistoryLogRepository, HistoryLogRepository>();
builder.Services.AddScoped<IContextService, ContextService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IKeyGenerator, KeyGenerator>();
builder.Services.AddSingleton<IRabbitMQProducerService, RabbitMQProducerService>();

builder.Services.AddMvc(options => options.ModelValidatorProviders.Clear());
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<AuthenMiddleware>();

app.MapControllers();

app.Run();
