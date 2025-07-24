using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Phone_Book.Enums;
using Phone_Book.Models;

namespace Phone_Book.Controllers;

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
        /*
         use Expression instead of Func because it's better
        for performance and creates a tree that EF can see
        through and know exactly what to retrieve instead of
        retrieving all the records and then using LINQ on it.
        */
        Expression<Func<Contact, bool>> func = queryType switch 
        {
            QueryType.PhoneNumber => (contact => contact.PhoneNumber == query),
            QueryType.Email => contact => contact.Email == query,
            QueryType.Name => contact => contact.Name == query,
        };

        return contactDbContext.Contacts
            .Where(func)
            .ToListAsync();
    }

    public async Task DeleteContact(Contact contact)
    {
        
        contactDbContext.Remove(contact);

        await contactDbContext.SaveChangesAsync();

    }

    public async Task UpdateContact(int id, QueryType updateType, string updateValue)
    {
        var contact = await GetContactById(id);

        switch (updateType)
        {
            case QueryType.Email:
                contact.Email = updateValue;
                break; 
            case QueryType.Name:
                contact.Name = updateValue;
                break;
            case QueryType.PhoneNumber:
                contact.PhoneNumber = updateValue;
                break;
        }

        await contactDbContext.SaveChangesAsync();

    }

    private async Task<Contact> GetContactById(int id)
    {
        var contact = await contactDbContext.Contacts
            .Where(contact => contact.Id == id)
            .FirstOrDefaultAsync();

        return contact;

    }

}