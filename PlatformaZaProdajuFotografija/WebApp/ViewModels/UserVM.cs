namespace WebApp.ViewModels;

public class UserVM
{
    public string Guid { get; set; } = null!;
    public string Username { get; set; } = null!;
    public List<string> Downloads { get; set; } = new();
}