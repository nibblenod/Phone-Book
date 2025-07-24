using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using Phone_Book.Models;

namespace Phone_Book.Helpers;

public static class Validator
{
    public static bool ContactsExist(List<Contact> contacts)
    {
        return (contacts.Count != 0);
    }
    public static bool ValidateEmail(string email)
    {
        return !email.IsNullOrEmpty() && Regex.IsMatch(email,@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    public static bool ValidateName(string name)
    {
        return (!name.IsNullOrEmpty());
    }

    public static bool ValidatePhoneNumber(string phoneNumber)
    {
        return Regex.IsMatch(phoneNumber, @"^\+?(\d{1,3})?[-.\s]?(\(?\d{3}\)?[-.\s]?)?(\d[-.\s]?){6,9}\d$");
    }
}