using VismaShortageManagement.Services;
using VismaShortageManagement.UI;

var dataService = new DataService("shortages.json");
var shortageService = new ShortageService(dataService);
var ui = new ConsoleUI(shortageService);

ui.Run();
