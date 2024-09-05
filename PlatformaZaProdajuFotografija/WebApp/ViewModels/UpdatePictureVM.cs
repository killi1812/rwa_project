using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApp.ViewModels;

public class UpdatePictureVM
{
    public string Guid { get; set; }

    [Display(Name = "Picture Name")]
    [Required(ErrorMessage = "There's not much sense of having a picture without the name, right?")]
    public string Name { get; set; }

    [Display(Name = "Description")] public string Description { get; set; }

    [Display(Name = "Photographer")]
    [ValidateNever]
    public string Photographer { get; set; }

    [Display(Name = "Tags")]
    [ValidateNever]
    public List<string> Tags { get; set; }
}