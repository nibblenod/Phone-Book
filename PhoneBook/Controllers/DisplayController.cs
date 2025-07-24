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
        Operation operationType = AskForSearchType(Enum.GetValues<Operation>(), "What do you want to do?");

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
            
        }
    }

    private async Task DeleteContactHandler()
    {
        AnsiConsole.Markup("Look up the contact that you want to delete...");
        Console.ReadKey();
        List<Contact> contacts = await SearchContactHandler();

        if (!Validator.ContactsExist(contacts)) return;
       
        
        int i = 0;
        Contact contact = AnsiConsole.Prompt(
            new SelectionPrompt<Contact>()
                .Title("Which contact do you want to delete? ")
                .AddChoices(contacts).UseConverter(contact => i++.ToString()));

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

    private async Task UpdateContactHandler()
    {
        throw new NotImplementedException();
    }

    private async Task AddContactHandler()
    {
        throw new NotImplementedException();
    }

    private async Task<List<Contact>> SearchContactHandler()
    {
            Console.WriteLine();
            List<Contact> results = new List<Contact>();
            QueryType searchType = AskForSearchType(Enum.GetValues<QueryType>(), "How do you want to search?");
            string query = AskForSearchQuery(searchType
                .GetAttribute<DisplayAttribute>()?
                .Name ?? searchType.ToString());

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

    
    
    private void PrintResults(List<Contact> results)
    {
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

    private TOption AskForSearchType<TOption>(TOption[] array, string title) where TOption : Enum
    {
        TOption searchType = AnsiConsole.Prompt(
            new SelectionPrompt<TOption>()
                .Title(title)
                .AddChoices(array).UseConverter(option => option.GetAttribute<DisplayAttribute>()?.Name ?? option.ToString()));
        return searchType;
    }
    

    private string AskForSearchQuery(string queryType)
    {
        var query = AnsiConsole.Ask<string>($"Search using {queryType}");
        
        return query.ToLower();
    }
    
}