using Spectre.Console;
using System.ComponentModel.DataAnnotations;
using Phone_Book.Enums;
using Phone_Book.Models;
using Phone_Book.Helpers;

namespace Phone_Book.Controllers;

public class DisplayController(ContactContext contactContext)

{
    private readonly ContactController contactController = new ContactController(contactContext);


    public async Task SearchHandler()
    {
        while (true)
        {
            Console.Clear();
            QueryType searchType = AskForSearchType(Enum.GetValues<QueryType>(), "How do you want to search?");
            string query = AskForSearchQuery(searchType
                .GetAttribute<DisplayAttribute>()?
                .Name ?? searchType.ToString());

            bool isValid = searchType switch
            {
                QueryType.Email => Phone_Book.Helpers.Validator.ValidateEmail(query),
                QueryType.PhoneNumber => Phone_Book.Helpers.Validator.ValidatePhoneNumber(query),
                QueryType.Name => Phone_Book.Helpers.Validator.ValidateName(query),
            };

            if (isValid)
            {
                List<Contact> results = await contactController.GetContact(query, searchType);
                PrintResults(results);
            }
            else AnsiConsole.Markup($"[red]Invalid {searchType
                .GetAttribute<DisplayAttribute>()?
                .Name ?? searchType.ToString()}[/]");

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();

        }

    }
    
    private void PrintResults(List<Contact> results)
    {
        if (results.Count == 0)
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