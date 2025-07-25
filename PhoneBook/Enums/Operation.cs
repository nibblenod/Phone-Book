using System.ComponentModel.DataAnnotations;

namespace Phone_Book.Enums;

public enum Operation
{
    [Display(Name="Search Contact")]
    Search,
    [Display(Name="Add Contact")]
    Add,
    [Display(Name="Update Contact")]
    Update,
    [Display(Name="Delete Contact")]
    Delete,
    Exit
}