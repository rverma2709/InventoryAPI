using App.API.Models;
using App.APIServices.Models;
using App.APIServices.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Root.Data.UnitOfWork;
using Root.Models.Utils;
using Root.Services.Common;
using Root.Services.DBContext;
using Root.Services.Interfaces;
using Root.Services.Services;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigurationManager Configuration = builder.Configuration;
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
AppConfig myConfig = Configuration.GetSection("AppConfig").Get<AppConfig>();
builder.Services.AddMemoryCache();

// Register MVC and Razor View Engine
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConfiguration>(Configuration);

builder.Services.AddCors();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddScoped<RequestLogger>();
builder.Services.AddScoped<ExceptionLogger>();

builder.Services.AddTransient<UnitOfWork<DBEntities>>();
builder.Services.AddTransient<ReadUOM<DBEntities>>();
builder.Services.AddTransient<WriteUOM<DBEntities>>();
builder.Services.AddScoped<ProcUOM>();
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddScoped<StaticService>();
builder.Services.AddScoped<APIStaticService>();
//builder.Services.AddScoped<ApplicantStaticService>();
builder.Services.AddTransient<ILogService, LogService>();
builder.Services.AddScoped(typeof(IDataService<,>), typeof(DataService<,>));
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<CacheUOM>();
//builder.Services.AddTransient<ICSCService, CSCService>();
builder.Services.AddTransient<IViewRenderService, ViewRenderService>();
builder.Services.AddScoped<UniqueKeyMatching>();
builder.Services.AddScoped<KeyGenrate>();
string RedisConnectionString = string.Empty;
if (myConfig.RedisConfig.IsEncConnectionString)
{
    RedisConnectionString = CommonLib.GetRedisPlainConnectionString(myConfig.RedisConfig.RedisConnectionString);
}
else
{
    RedisConnectionString = myConfig.RedisConfig.RedisConnectionString;
}
ConfigurationOptions configurationOption = ConfigurationOptions.Parse(RedisConnectionString);
configurationOption.ClientName = myConfig.tokenConfigs.ChannelId;
CacheService.configurationOption = configurationOption;
CacheService.channelConfigs = myConfig.ChannelConfig;
CacheService.parentChannelID = CacheService.parentChannelID = (CacheChannels)Enum.Parse(typeof(CacheChannels), myConfig.tokenConfigs.ChannelId, true);
string ChannelID = myConfig.tokenConfigs.ChannelId;

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

// Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()          // Allow any origin
                  .AllowAnyHeader()          // Allow any header
                  .AllowAnyMethod()          // Allow any method (GET, POST, etc.)
                  //.AllowCredentials()        // Allow credentials (if needed)
                  .WithExposedHeaders("X-Unique-Key");  // Expose specific headers
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");  // Apply the CORS policy

app.UseAuthorization();



app.MapControllers();

app.Run();
