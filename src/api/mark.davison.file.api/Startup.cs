namespace mark.davison.file.api;

[UseCQRSServer(typeof(DtosRootType), typeof(CommandsRootType), typeof(QueriesRootType))]
public sealed class Startup
{
    public IConfiguration Configuration { get; }

    public AppSettings AppSettings { get; set; } = null!;

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        AppSettings = services.ConfigureSettingsServices<AppSettings>(Configuration);
        if (AppSettings == null) { throw new InvalidOperationException(); }

        Console.WriteLine(AppSettings.DumpAppSettings(AppSettings.PRODUCTION_MODE));

        services
            .AddCors()
            .AddLogging()
            .UseCookieOidcAuth(
                AppSettings.AUTH,
                AppSettings.CLAIMS,
                _ => { },
                _ => { },
                _ => { },
                _ => { },
                AppSettings.API_ORIGIN)
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddHttpContextAccessor()
            .AddAuthorization()
            .AddHealthCheckServices<InitializationHostedService>()
            .AddScoped<ICurrentUserContext, CurrentUserContext>()
            .AddDatabase<FileDbContext>(AppSettings.PRODUCTION_MODE, AppSettings.DATABASE, typeof(SqliteContextFactory), typeof(PostgresContextFactory))
            .AddCoreDbContext<FileDbContext>()
            .AddSingleton<IDateService>(new DateService(DateService.DateMode.Utc))
            .AddCQRSServer()
            .AddHttpClient()
            .AddHttpContextAccessor()
            .AddRedis(AppSettings.REDIS, AppSettings.SECTION, AppSettings.PRODUCTION_MODE);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCors(builder =>
            builder
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .SetIsOriginAllowed(_ => true) // TODO: Config driven
                .AllowAnyMethod()
                .AllowCredentials()
                .AllowAnyHeader());

        app.UseHttpsRedirection();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app
            .UseMiddleware<RequestResponseLoggingMiddleware>()
            .UseRouting()
            .UseAuthentication()
            .UseMiddleware<CheckAccessTokenValidityMiddleware>()
            .UseAuthorization()
            .UseMiddleware<PopulateUserContextMiddleware>()
            .UseMiddleware<ValidateUserExistsInDbMiddleware>()
            .UseEndpoints(endpoints =>
            {
                endpoints
                    .MapHealthChecks()
                    .UseAuthEndpoints(AppSettings.WEB_ORIGIN)
                    .MapGet<User>()
                    .MapGetById<User>()
                    .MapPost<User>()
                    .MapCQRSEndpoints();

                if (!AppSettings.PRODUCTION_MODE)
                {

                }
            });
    }
}
