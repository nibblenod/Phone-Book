using System.ComponentModel.DataAnnotations;

namespace Phone_Book;

public enum QueryType
{
    [Display(Name = "Phone Number")]
    PhoneNumber,
    Name,
    Email,
}