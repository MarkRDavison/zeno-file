namespace mark.davison.file.api.Middleware;

public sealed class ValidateUserExistsInDbMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AppSettings _appSettings;
    private static object _lock = new();

    public ValidateUserExistsInDbMiddleware(
        RequestDelegate next,
        IOptions<AppSettings> appSettings)
    {
        _next = next;
        _appSettings = appSettings.Value;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!_appSettings.PRODUCTION_MODE)
        {
            var currentUserContext = context.RequestServices.GetRequiredService<ICurrentUserContext>();
            if (currentUserContext.CurrentUser != null)
            {
                bool lockTaken = false;
                try
                {
                    var dbContext = context.RequestServices.GetRequiredService<IDbContext>();

                    if (await dbContext.GetByIdAsync<User>(currentUserContext.CurrentUser.Id, CancellationToken.None) is null)
                    {
                        Monitor.Enter(_lock, ref lockTaken);

                        await dbContext.UpsertEntityAsync(currentUserContext.CurrentUser, CancellationToken.None);

                        var userDefaulter = context.RequestServices.GetService<IEntityDefaulter<User>>();

                        if (userDefaulter is not null)
                        {
                            await userDefaulter.DefaultAsync(currentUserContext.CurrentUser, currentUserContext.CurrentUser);
                        }

                        await dbContext.SaveChangesAsync(CancellationToken.None);
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        Monitor.Exit(_lock);
                    }
                }
            }
        }

        await _next(context);
    }
}