using System.ComponentModel.DataAnnotations;

namespace webAPIAutores.DTOs;

public class EditarAdminDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}