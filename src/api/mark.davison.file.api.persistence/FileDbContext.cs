namespace mark.davison.file.api.persistence;

public sealed class FileDbContext : DbContextBase<FileDbContext>
{
    public FileDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserEntityConfiguration).Assembly);
    }

    public DbSet<User> Users => Set<User>();
}
