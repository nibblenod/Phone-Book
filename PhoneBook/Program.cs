using Phone_Book;
using Phone_Book.Controllers;

using var contactContext = new ContactContext();

DisplayController displayController = new DisplayController(contactContext);
await displayController.MainMenuHandler();