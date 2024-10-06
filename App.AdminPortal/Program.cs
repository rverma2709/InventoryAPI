using App.AdminPortalServices.Models;
using App.AdminPortalServices.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.FileProviders;
using Root.Data.UnitOfWork;
using Root.Models.Config;
using Root.Models.Utils;
using Root.Services.Common;
using Root.Services.DBContext;
using Root.Services.Interfaces;
using Root.Services.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager Configuration = builder.Configuration;

AppConfig myConfig = Configuration.GetSection("AppConfig").Get<AppConfig>();
// Add services to the container.
builder.Services.AddAuthorization();


// Add services to the container.
builder.Services.AddMvc();
//builder.Services.AddSingleton<IFileProvider>(
//new PhysicalFileProvider(CommonLib.wwwRootPath));
//builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

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
builder.Services.AddScoped<AdminPortalStaticService>();
//builder.Services.AddScoped<ApplicantStaticService>();
builder.Services.AddTransient<ILogService, LogService>();
builder.Services.AddScoped(typeof(IDataService<,>), typeof(DataService<,>));
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<CacheUOM>();
//builder.Services.AddTransient<ICSCService, CSCService>();
builder.Services.AddTransient<IViewRenderService, ViewRenderService>();

//builder.Services.AddTransient<IHomeService, HomeService>();

//builder.Services.AddTransient<IApplicantLoginService, ApplicantLoginService>();
//builder.Services.AddTransient<IApplySchemeService, ApplySchemeService>();
//builder.Services.AddTransient<IScanApplicationsService, ScanApplicationsService>();
//builder.Services.AddTransient<IAadhaarService, AadhaarService>();
//builder.Services.AddTransient<IGPActivationService, GPActivationService>();
string RedisConnectionString = string.Empty;
//if (myConfig.RedisConfig.IsEncConnectionString)
//{
//    RedisConnectionString = CommonLib.GetRedisPlainConnectionString(myConfig.RedisConfig.RedisConnectionString);
//}
//else
//{
//    RedisConnectionString = myConfig.RedisConfig.RedisConnectionString;
//}
//ConfigurationOptions configurationOption = ConfigurationOptions.Parse(RedisConnectionString);
//configurationOption.ClientName = myConfig.tokenConfigs.ChannelId;
//CacheService.configurationOption = configurationOption;
//CacheService.channelConfigs = myConfig.ChannelConfig;
//CacheService.parentChannelID = CacheService.parentChannelID = (CacheChannels)Enum.Parse(typeof(CacheChannels), myConfig.tokenConfigs.ChannelId, true);
string ChannelID = myConfig.tokenConfigs.ChannelId;
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(myConfig.sessionConfigs.SessionExpiry);
    options.Cookie.Name = ChannelID + "_";
});







var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseSession();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
