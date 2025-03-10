using AutoMapper;
using BPHN.BusinessLayer.ImpServices;
using BPHN.BusinessLayer.ImpServices.MailBuilders;
using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.ImpRepositories;
using BPHN.DataLayer.IRepositories;
using BPHN.ImpRabbitMQLayer;
using BPHN.IRabbitMQLayer;
using BPHN.ModelLayer;
using BPHN.WebAPI;
using BPHN.WebAPI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var appSettings = builder.Configuration.GetSection("AppSettings");

// Add services to the container.
builder.Services.Configure<AppSettings>(appSettings);
builder.Services.AddHttpContextAccessor();

builder.Services.AddStackExchangeRedisCache(options =>
{
	options.Configuration = appSettings.GetValue<string>("RedisCacheUrl");
});
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IPitchService, PitchService>();
builder.Services.AddScoped<IPitchRepository, PitchRepository>();
builder.Services.AddScoped<IHistoryLogService, HistoryLogService>();
builder.Services.AddScoped<IHistoryLogRepository, HistoryLogRepository>();
builder.Services.AddScoped<IContextService, ContextService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IKeyGenerator, KeyGenerator>();
builder.Services.AddScoped<IConfigService, ConfigService>();
builder.Services.AddScoped<IConfigRepository, ConfigRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IBookingDetailService, BookingDetailService>();
builder.Services.AddScoped<ITimeFrameInfoRepository, TimeFrameInfoRepository>();
builder.Services.AddScoped<ITimeFrameInfoService, TimeFrameInfoService>();
builder.Services.AddScoped<IBookingDetailRepository, BookingDetailRepository>();
builder.Services.AddScoped<IBookingDetailService, BookingDetailService>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddSingleton<IRabbitMQProducerService, RabbitMQProducerService>();
builder.Services.AddSingleton<IResourceService, ResourceService>();
builder.Services.AddSingleton<IGlobalVariableService, GlobalVariableService>();
builder.Services.AddSingleton<IKeyGenerator, KeyGenerator>();
builder.Services.AddSingleton<IMailBuilderFactory, MailBuilderFactory>();
builder.Services.AddSingleton<IMailBuilder, ForgotPasswordMailBuilder>();
builder.Services.AddSingleton<IMailBuilder, SetPasswordMailBuilder>();
builder.Services.AddSingleton<IMailBuilder, DeclineBookingMailBuilder>();
builder.Services.AddSingleton<IMailBuilder, ApprovalBookingMailBuilder>();
builder.Services.AddSingleton(new MapperConfiguration(mc =>
{
	mc.AddProfile(new MappingProfiles());
}).CreateMapper());
builder.Services.AddMvc(options => options.ModelValidatorProviders.Clear());
builder.Services.AddControllers();

Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.WriteTo.File("C://bphn/log-.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day)
				.CreateLogger();

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
builder.Services.AddSignalR();
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
	options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
	.AddCookie()
	.AddGoogle(googleOptions =>
	{
		googleOptions.ClientId = "1069130122771-l2vls4cofg16runiou4hlaq3n3s74b0i.apps.googleusercontent.com";
		googleOptions.ClientSecret = "hq542hbiI9zifILsWchgT8xS";
		googleOptions.CallbackPath = "/signin-google"; // This should match the redirect URI set in the Google Console
	});

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI();

app.UseCors(options => options.WithOrigins("*").AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();

app.UseStaticFiles();

//app.UseAuthentication();
//app.UseAuthorization();
app.UseMiddleware<AuthenMiddleware>();

app.MapControllers();

app.MapHub<WsReceiveService>("/ws");

app.Run();
