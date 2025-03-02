namespace mark.davison.file.web.components.Ignition;

public static class DependecyInjectionExtensions
{
    public static IServiceCollection UseFileComponents(
    this IServiceCollection services,
        IAuthenticationConfig authConfig)
    {
        services.UseCommonClient(authConfig, typeof(Routes));
        return services;
    }
}