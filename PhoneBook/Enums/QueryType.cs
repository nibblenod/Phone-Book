using System.ComponentModel.DataAnnotations;

namespace Phone_Book.Enums;

public enum QueryType
{
    [Display(Name = "Phone Number")]
    PhoneNumber,
    Name,
    Email,
}