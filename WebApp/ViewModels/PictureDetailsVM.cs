using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApp.ViewModels;

public class PictureDetailsVM
{
    public string Guid { get; set; }

    [Display(Name = "Picture Name")]
    [Required(ErrorMessage = "There's not much sense of having a picture without the name, right?")]
    public string Name { get; set; }

    [Display(Name = "Description")] public string Description { get; set; }

    [Display(Name = "Owner")]
    [ValidateNever]
    public string Username { get; set; }

    public string UserGuid { get; set; }

    [Display(Name = "Photographer")]
    [ValidateNever]
    public string Photographer { get; set; }

    //TODO check if this can be used for idsplaing a picture
    public string? Url { get; set; }

    [Display(Name = "Tags")]
    [ValidateNever]
    public List<string> Tags { get; set; }

    [Display(Name = "Downloads Count")] public int DownloadsCount { get; set; }

    [Display(Name = "Recent Downloads")] public List<string> Downloads { get; set; }
}