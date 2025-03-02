namespace mark.davison.file.api.migrations.postgres;

[ExcludeFromCodeCoverage]
[DatabaseMigrationAssembly(DatabaseType.Postgres)]
public sealed class PostgresContextFactory : PostgresDbContextFactory<FileDbContext>
{
    protected override string ConfigName => "DATABASE";

    protected override FileDbContext DbContextCreation(
            DbContextOptions<FileDbContext> options
        ) => new FileDbContext(options);
}
