using System;
using System.Globalization;
using VismaShortageManagement.Models;
using VismaShortageManagement.Services;

namespace VismaShortageManagement.UI
{
    public class ConsoleUI
    {
        private readonly IShortageService _shortageService;
        private User _currentUser;

        public ConsoleUI(IShortageService shortageService)
        {
            _shortageService = shortageService;
        }

        public void Run()
        {
            Console.WriteLine("=== Visma Resource Shortage Management System ===");
            Login();

            while (true)
            {
                ShowMenu();
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RegisterShortage();
                        break;
                    case "2":
                        DeleteShortage();
                        break;
                    case "3":
                        ListShortages();
                        break;
                    case "4":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        private void Login()
        {
            Console.Write("Enter your name: ");
            var name = Console.ReadLine();
            
            Console.Write("Are you an administrator? (y/n): ");
            var isAdmin = Console.ReadLine()?.ToLower() == "y";
            
            _currentUser = new User(name, isAdmin);
            Console.WriteLine($"Welcome, {_currentUser.Name}!");
            if (_currentUser.IsAdministrator)
                Console.WriteLine("You have administrator privileges.");
        }

        private void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine($"=== Logged in as: {_currentUser.Name} {(_currentUser.IsAdministrator ? "(Admin)" : "")} ===");
            Console.WriteLine("1. Register new shortage");
            Console.WriteLine("2. Delete shortage");
            Console.WriteLine("3. List shortages");
            Console.WriteLine("4. Exit");
            Console.Write("Choose an option: ");
        }

        private void RegisterShortage()
        {
            Console.Clear();
            Console.WriteLine("=== Register New Shortage ===");

            Console.Write("Title: ");
            var title = Console.ReadLine();

            var room = SelectRoom();
            var category = SelectCategory();

            Console.Write("Priority (1-10): ");
            if (!int.TryParse(Console.ReadLine(), out int priority) || priority < 1 || priority > 10)
            {
                Console.WriteLine("Invalid priority. Must be between 1 and 10.");
                return;
            }

            var shortage = new Shortage(title, _currentUser.Name, room, category, priority);
            
            if (_shortageService.RegisterShortage(shortage))
            {
                Console.WriteLine("Shortage registered successfully!");
            }
        }

        private void DeleteShortage()
        {
            Console.Clear();
            Console.WriteLine("=== Delete Shortage ===");

            Console.Write("Title of shortage to delete: ");
            var title = Console.ReadLine();

            var room = SelectRoom();

            if (_shortageService.DeleteShortage(title, room, _currentUser.Name, _currentUser.IsAdministrator))
            {
                Console.WriteLine("Shortage deleted successfully!");
            }
        }

        private void ListShortages()
        {
            Console.Clear();
            Console.WriteLine("=== List Shortages ===");

            Console.Write("Filter by title (optional): ");
            var titleFilter = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(titleFilter)) titleFilter = null;

            Console.Write("Filter from date (yyyy-mm-dd, optional): ");
            DateTime? fromDate = null;
            var fromDateStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(fromDateStr))
            {
                if (DateTime.TryParseExact(fromDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedFromDate))
                {
                    fromDate = parsedFromDate;
                }
            }

            Console.Write("Filter to date (yyyy-mm-dd, optional): ");
            DateTime? toDate = null;
            var toDateStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(toDateStr))
            {
                if (DateTime.TryParseExact(toDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedToDate))
                {
                    toDate = parsedToDate;
                }
            }

            Category? categoryFilter = SelectCategoryOptional();
            Room? roomFilter = SelectRoomOptional();

            var shortages = _shortageService.GetShortages(_currentUser.Name, _currentUser.IsAdministrator, 
                titleFilter, fromDate, toDate, categoryFilter, roomFilter);

            Console.WriteLine($"\n=== Found {shortages.Count} shortages ===");
            foreach (var shortage in shortages)
            {
                Console.WriteLine($"- {shortage}");
            }
        }

        private Room SelectRoom()
        {
            Console.WriteLine("Select room:");
            Console.WriteLine("1. Meeting Room");
            Console.WriteLine("2. Kitchen");
            Console.WriteLine("3. Bathroom");
            Console.Write("Choice: ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= 3)
            {
                return (Room)choice;
            }

            Console.WriteLine("Invalid choice, defaulting to Meeting Room.");
            return Room.MeetingRoom;
        }

        private Room? SelectRoomOptional()
        {
            Console.WriteLine("Filter by room (optional):");
            Console.WriteLine("1. Meeting Room");
            Console.WriteLine("2. Kitchen");
            Console.WriteLine("3. Bathroom");
            Console.WriteLine("Press Enter to skip");
            Console.Write("Choice: ");

            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return null;

            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= 3)
            {
                return (Room)choice;
            }

            return null;
        }

        private Category SelectCategory()
        {
            Console.WriteLine("Select category:");
            Console.WriteLine("1. Electronics");
            Console.WriteLine("2. Food");
            Console.WriteLine("3. Other");
            Console.Write("Choice: ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= 3)
            {
                return (Category)choice;
            }

            Console.WriteLine("Invalid choice, defaulting to Other.");
            return Category.Other;
        }

        private Category? SelectCategoryOptional()
        {
            Console.WriteLine("Filter by category (optional):");
            Console.WriteLine("1. Electronics");
            Console.WriteLine("2. Food");
            Console.WriteLine("3. Other");
            Console.WriteLine("Press Enter to skip");
            Console.Write("Choice: ");

            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return null;

            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= 3)
            {
                return (Category)choice;
            }

            return null;
        }
    }
}
