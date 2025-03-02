namespace mark.davison.file.web.ui.Ignition;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseFileWeb(this IServiceCollection services, IAuthenticationConfig authConfig)
    {
        services
            .UseFileComponents(authConfig)
            .UseFluxorState(typeof(Program), typeof(FeaturesRootType))
            .UseClientRepository(WebConstants.ApiClientName, WebConstants.LocalBffRoot);

        return services;
    }
}
