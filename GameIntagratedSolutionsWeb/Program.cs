using IntelliTrackSolutionsWeb.EFModels;
using IntelliTrackSolutionsWeb.EFModels_Partial;
using IntelliTrackSolutionsWeb.Middleware;
using IntelliTrackSolutionsWeb.Middleware.AuthorizationPolicy;
using IntelliTrackSolutionsWeb.SignalRHub;
using IntelliTrackSolutionsWeb.SignalRHub.SignalRUserAndGroupSettings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using AuthorizationMiddleware = IntelliTrackSolutionsWeb.Middleware.AuthorizationMiddleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ServiceSystemContext>(o => o.ConfigurationMySql());

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.Name = ".IntelliTrackSolutions.Session";
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddScheme<BasicAuthenticationOptions, BasicAuthenticationMiddleware>("Basic", options => { })
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

        options.LoginPath = "/Users/Login";
        options.LogoutPath = "/Users/Login";
        options.AccessDeniedPath = "/Home/ErrorCodePage?errorCode=403";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddSingleton<IAuthorizationHandler, OrganizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, OrganizationAccessHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.Requirements.Add(new OrganizationAccessRequirement(1, new[] { 5, 6, 7, 8, 9, 10 })));

    options.AddPolicy("OrganizationPolicy", policy =>
        policy.Requirements.Add(new OrganizationRequirement()));

    options.AddPolicy("OrganizationAdministrationPolicy", policy =>
        policy.Requirements.Add(new OrganizationRequirement(new[] { 5, 6, 7, 8, 9, 10 })));
});

builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

builder.Services.AddSignalR(hubOptionsDefault =>
{
    hubOptionsDefault.EnableDetailedErrors = true;
    hubOptionsDefault.KeepAliveInterval = TimeSpan.FromMinutes(30);
    hubOptionsDefault.HandshakeTimeout = TimeSpan.FromMinutes(30);
    hubOptionsDefault.MaximumParallelInvocationsPerClient = 100;
    hubOptionsDefault.ClientTimeoutInterval = TimeSpan.FromMinutes(30);
    hubOptionsDefault.StreamBufferCapacity = 30000;
});

builder.Services.AddCors();
builder.Services.AddMvc(options => options.EnableEndpointRouting = false);
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
else
{
    app.UseCors(x => x
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(_ => true)
        .AllowCredentials());
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithRedirects("/Home/ErrorCodePage?errorCode={0}");

app.UseSession();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.UseMiddleware<AuthorizationMiddleware>(app.Services);

app.UseMvcWithDefaultRoute();

app.MapHub<RealApiDeviceHub>("/api",
    options =>
    {
        options.ApplicationMaxBufferSize = 30000;
        options.TransportMaxBufferSize = 30000;
        options.TransportSendTimeout = TimeSpan.FromMinutes(30);
        options.WebSockets.CloseTimeout = TimeSpan.FromMinutes(30);
        options.Transports = HttpTransportType.WebSockets;
    });

app.Run();