namespace mark.davison.file.shared.models;

public class FileEntity : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
}