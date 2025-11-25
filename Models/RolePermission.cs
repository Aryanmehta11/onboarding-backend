// Models/RolePermission.cs
public class RolePermission {
    public int Id { get; set; }
    public int RoleId { get; set; }
    public int SectionId { get; set; }
    public bool CanView { get; set; }
    public bool CanEdit { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
