public class Section
{
    public int Id { get; set; }
    public string KeyName { get; set; }
    public string DisplayName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}