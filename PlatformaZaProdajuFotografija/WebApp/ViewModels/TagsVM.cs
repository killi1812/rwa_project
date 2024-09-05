using Data.Models;

namespace WebApp.ViewModels;

public class TagVM
{
    public string Guid { get; set; }
    public string Name { get; set; }
    public int PicturesCount { get; set; }
}