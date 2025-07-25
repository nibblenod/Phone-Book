using Spectre.Console;
using System.ComponentModel.DataAnnotations;
using Phone_Book.Enums;
using Phone_Book.Models;
using Phone_Book.Helpers;
using Validator = Phone_Book.Helpers.Validator;

namespace Phone_Book.Controllers;

public class DisplayController(ContactContext contactContext)

{
    private readonly ContactController contactController = new ContactController(contactContext);


    public async Task MainMenuHandler()
    {
        bool exitApp = false;
        while (!exitApp)
        {
            Console.Clear();
            Operation operationType = AskForType(Enum.GetValues<Operation>(), "What do you want to do?");

            switch (operationType)
            {
                case Operation.Search:
                    await SearchContactHandler();
                    break;
                case Operation.Add:
                    await AddContactHandler();
                    break;
                case Operation.Update:
                    await UpdateContactHandler();
                    break;
                case Operation.Delete:
                    await DeleteContactHandler();
                    break;
                case Operation.Exit:
                    exitApp = true;
                    break;
            } 
        }
    }

    private async Task DeleteContactHandler()
    {
        Console.WriteLine();
        
        Contact? contact = await FindAndSelectContact(Operation.Delete);
        if (contact != null)
        {
            try
            {
                await contactController.DeleteContact(contact);
                AnsiConsole.Markup("[green]Contact deleted successfully![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup($"[red]Contact couldn't be deleted![/] Error: {ex.Message}");
            }
        }

        Console.ReadKey();
    }

    private async Task UpdateContactHandler()
    {
        
        Contact? contact = await FindAndSelectContact(Operation.Search);
    
        if (contact != null)
        {
            QueryType updateType = AskForType(Enum.GetValues<QueryType>(), "What do you want to update?");
            string updateValue = AnsiConsole.Ask<string>("Update Value: ");
            try
            {
                await contactController.UpdateContact(contact, updateType, updateValue);
                AnsiConsole.Markup("[green]Contact updated successfully![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup($"[red]Contact could not be updated![/] Error: {ex.Message}");
            }
        }

        Console.ReadKey();

    }
    
    private async Task AddContactHandler()
    {
        
        Contact contact = CreateContact();
        
        try
        {
            await contactController.AddContact(contact);
            AnsiConsole.Markup("[green]Contact added successfully![/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.Markup($"[red]Couldn't add contact![/] Error: {ex.Message}");
        }

        Console.ReadKey();
    }

    private async Task<List<Contact>> SearchContactHandler()
    {
        
        List<Contact> results = new List<Contact>();
            
        QueryType searchType = AskForType(Enum.GetValues<QueryType>(), "How do you want to search?");
            
        string query = AskForQuery(searchType);

        bool isValid = searchType switch
        {
            QueryType.Email => Validator.ValidateEmail(query),
            QueryType.PhoneNumber => Validator.ValidatePhoneNumber(query),
            QueryType.Name => Validator.ValidateName(query),
        };

        if (isValid)
        {
            try
            {
                results = await contactController.GetContact(query, searchType);
                PrintResults(results);
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup($"[red]Couldn't retrieve contacts![/] Error: {ex.Message}");
            }
        }
        else AnsiConsole.Markup($"[red]Invalid {searchType
            .GetAttribute<DisplayAttribute>()?
            .Name ?? searchType.ToString()}[/]");

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();

        return results;
    }

    private async Task<Contact?> FindAndSelectContact(Operation operation)
    {
        Contact? contact = null;
        string operationKeyword = operation.ToString().ToLower();
        
        AnsiConsole.Markup($"Look up the contact that you want to {operationKeyword}...");
        Console.ReadKey();
        List<Contact> contacts = await SearchContactHandler();

        if (Validator.ContactsExist(contacts))
        {
            int i = 0;
            contact = AnsiConsole.Prompt(
                new SelectionPrompt<Contact>()
                    .Title("Which contact do you want to delete? ")
                    .AddChoices(contacts).UseConverter(contact => i++.ToString()));
        }

        return contact;

    }
    
    private void PrintResults(List<Contact> results)
    {
        Console.WriteLine();
        
        if (!Validator.ContactsExist(results))
        {
            AnsiConsole.Markup("[red]No contacts found![/]");
            return;
        }
        
        Table table = new Table();

        table.AddColumn("No.");
        table.AddColumn("Name");
        table.AddColumn("Phone Number");
        table.AddColumn("Email");

        int serialNumber = 1;
        foreach (Contact contact in results)
        {
            table.AddRow(serialNumber.ToString(),contact.Name,contact.PhoneNumber,contact.Email);
            
            serialNumber++;
        }
        
        AnsiConsole.Write(table);
    }
  
    private Contact CreateContact()
    {
        
        string name = string.Empty;
        while (!Validator.ValidateName(name))
        {
            name = AskForQuery(QueryType.Name);
        }

        string email = string.Empty;
        while (!Validator.ValidateEmail(email))
        {
            email = AskForQuery(QueryType.Email);
        }

        string phoneNumber = string.Empty;
        while (!Validator.ValidatePhoneNumber(phoneNumber))
        {
            phoneNumber = AskForQuery(QueryType.PhoneNumber);
        }

        return new Contact()
        {
            Name = name,
            Email = email,
            PhoneNumber = phoneNumber,
        };
    }

    private string AskForQuery(QueryType query)
    {
        
        string message = query switch
        {
            QueryType.Name => "Enter name of the contact: [yellow]Should not be blank[/]\n",
            QueryType.Email => "Enter email of the contact: [yellow]Format-> name@example.com[/]\n",
            QueryType.PhoneNumber =>
                "Enter phone number of the contact:\n [yellow]Use digits only, with optional spaces, dashes," +
                " dots, or parentheses. Country code is optional.[/]\n",
        };

        return AnsiConsole.Ask<string>(message);
    }
    
    private TOption AskForType<TOption>(TOption[] array, string title) where TOption : Enum
    {
        
        TOption searchType = AnsiConsole.Prompt(
            new SelectionPrompt<TOption>()
                .Title(title)
                .AddChoices(array).UseConverter(option => option.GetAttribute<DisplayAttribute>()?.Name ?? option.ToString()));
        return searchType;
    }
    
}