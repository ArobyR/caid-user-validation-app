using System.ComponentModel.DataAnnotations;
using Microsoft.Build.Framework;

namespace UserAuth.Models;
public class LoginModel
{
    [EmailAddress]
    public required string Email { get; set; }
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}