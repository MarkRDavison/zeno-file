namespace mark.davison.file.api.models.configuration;

public abstract class FileEntityConfiguration<T> : IEntityTypeConfiguration<T>
    where T : FileEntity
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder
            .HasKey(e => e.Id);

        builder
            .Property(e => e.Id)
            .ValueGeneratedNever();

        builder
            .Property(_ => _.Created);
        builder
            .Property(_ => _.LastModified);

        builder
            .HasOne(_ => _.User)
            .WithMany()
            .HasForeignKey(_ => _.UserId);

        if (!ConfigureNavigationManually)
        {
            NavigationPropertyEntityConfigurations.ConfigureEntity(builder);
        }

        ConfigureEntity(builder);
    }

    public const int NameMaxLength = 255;
    public const int PeriodMaxLength = 63;

    protected virtual bool ConfigureNavigationManually => false;

    public abstract void ConfigureEntity(EntityTypeBuilder<T> builder);
}