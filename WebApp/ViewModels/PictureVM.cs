using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApp.ViewModels;

public class PictureVM
{
    public int Id { get; set; }
    public string Guid { get; set; }

    [Display(Name = "Picture Name")]
    [Required(ErrorMessage = "There's not much sense of having a picture without the name, right?")]
    public string Name { get; set; }

    [Display(Name = "Select Artist")] public int ArtistId { get; set; }

    [Display(Name = "Photographer")]
    [ValidateNever]
    public string Photographer { get; set; }

    //TODO check if this can be used for idsplaing a picture
    public string? Url { get; set; }

    [Display(Name = "Tags")]
    [ValidateNever]
    public List<string> Tags { get; set; }
}