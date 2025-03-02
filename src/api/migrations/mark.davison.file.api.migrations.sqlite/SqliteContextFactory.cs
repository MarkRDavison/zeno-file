namespace mark.davison.file.api.migrations.sqlite;

[ExcludeFromCodeCoverage]
[DatabaseMigrationAssembly(DatabaseType.Sqlite)]
public sealed class SqliteContextFactory : SqliteDbContextFactory<FileDbContext>
{
    protected override FileDbContext DbContextCreation(
            DbContextOptions<FileDbContext> options
        ) => new FileDbContext(options);
}
