using Phone_Book;

using var contactContext = new ContactContext();

DisplayController displayController = new DisplayController(contactContext);
await displayController.MenuHandler();