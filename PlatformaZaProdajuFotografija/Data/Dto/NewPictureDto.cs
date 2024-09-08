using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Data.Dto;

public class NewPictureDto
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Photographer is required")]
    public string Photographer { get; set; }

    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Data is required")]
    public IFormFile Data { get; set; }

    [Required(ErrorMessage = "At least one tag is required")]
    public string Tags { get; set; }
}