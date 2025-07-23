using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Phone_Book.Models;
namespace Phone_Book;

public class ContactController(ContactContext contactDbContext)
{
    private readonly ContactContext contactDbContext = contactDbContext;
    
    public async Task AddContact(string email, string phoneNumber, string name)
    {
        Contact contactToAdd = new Contact()
        {
            Email = email,
            PhoneNumber = phoneNumber,
            Name = name,
        };
        contactDbContext.Contacts.Add(contactToAdd);

        await contactDbContext.SaveChangesAsync();
    }

    public Task<List<Contact>> GetContact(string query, QueryType queryType)
    {
        Expression<Func<Contact, bool>> func = queryType switch
        {
            QueryType.PhoneNumber => (contact => contact.PhoneNumber == query),
            QueryType.Email => contact => contact.Email == query,
            QueryType.Name => contact => contact.Name == query,
        };

        return contactDbContext.Contacts.Where(func).ToListAsync();
    }

  
    
}