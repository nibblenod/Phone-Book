using Spectre.Console;
using System.ComponentModel.DataAnnotations;
using Phone_Book.Models;


namespace Phone_Book;

public class DisplayController(ContactContext contactContext)

{
    private readonly ContactController contactController = new ContactController(contactContext);
    
    public async Task MenuHandler()
    {
        while (true)
        {
            Console.Clear();
            QueryType searchType = AskForSearchType();
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
    
    private QueryType AskForSearchType()
    {
        QueryType searchType = AnsiConsole.Prompt(
            new SelectionPrompt<QueryType>()
                .Title("How do you want to search?")
                .AddChoices(Enum.GetValues<QueryType>()).UseConverter(queryType => queryType.GetAttribute<DisplayAttribute>()?.Name ?? queryType.ToString()));
        return searchType;
    }

    private string AskForSearchQuery(string queryType)
    {
        var query = AnsiConsole.Ask<string>($"Search using {queryType}");
        
        return query.ToLower();
    }
    
}