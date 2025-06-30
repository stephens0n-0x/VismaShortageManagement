using System;
using System.Collections.Generic;
using System.Linq;
using VismaShortageManagement.Models;

namespace VismaShortageManagement.Services
{
    public class ShortageService : IShortageService
    {
        private readonly IDataService _dataService;
        private List<Shortage> _shortages;

        public ShortageService(IDataService dataService)
        {
            _dataService = dataService;
            _shortages = _dataService.LoadShortages();
        }

        public bool RegisterShortage(Shortage newShortage)
        {
            var existingShortage = _shortages.FirstOrDefault(s => 
                s.Title.Equals(newShortage.Title, StringComparison.OrdinalIgnoreCase) && 
                s.Room == newShortage.Room);

            if (existingShortage != null)
            {
                if (newShortage.Priority > existingShortage.Priority)
                {
                    _shortages.Remove(existingShortage);
                    _shortages.Add(newShortage);
                    _dataService.SaveShortages(_shortages);
                    Console.WriteLine($"Warning: Shortage already exists but priority was higher. Updated shortage: {newShortage.Title} in {newShortage.Room}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"Warning: Shortage already exists: {newShortage.Title} in {newShortage.Room}. New priority ({newShortage.Priority}) is not higher than existing ({existingShortage.Priority}).");
                    return false;
                }
            }

            _shortages.Add(newShortage);
            _dataService.SaveShortages(_shortages);
            return true;
        }

        public bool DeleteShortage(string title, Room room, string userName, bool isAdmin)
        {
            var shortage = _shortages.FirstOrDefault(s => 
                s.Title.Equals(title, StringComparison.OrdinalIgnoreCase) && 
                s.Room == room);

            if (shortage == null)
            {
                Console.WriteLine("Shortage not found.");
                return false;
            }

            if (!isAdmin && !shortage.Name.Equals(userName, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("You can only delete shortages you created or you need administrator privileges.");
                return false;
            }

            _shortages.Remove(shortage);
            _dataService.SaveShortages(_shortages);
            return true;
        }

        public List<Shortage> GetShortages(string userName, bool isAdmin, string titleFilter = null, 
            DateTime? fromDate = null, DateTime? toDate = null, Category? categoryFilter = null, 
            Room? roomFilter = null)
        {
            var query = _shortages.AsQueryable();

            // User access control
            if (!isAdmin)
            {
                query = query.Where(s => s.Name.Equals(userName, StringComparison.OrdinalIgnoreCase));
            }

            // Apply filters
            if (!string.IsNullOrEmpty(titleFilter))
            {
                query = query.Where(s => s.Title.Contains(titleFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(s => s.CreatedOn.Date >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                query = query.Where(s => s.CreatedOn.Date <= toDate.Value.Date);
            }

            if (categoryFilter.HasValue)
            {
                query = query.Where(s => s.Category == categoryFilter.Value);
            }

            if (roomFilter.HasValue)
            {
                query = query.Where(s => s.Room == roomFilter.Value);
            }

            // Sort by priority descending (high priority first)
            return query.OrderByDescending(s => s.Priority).ThenByDescending(s => s.CreatedOn).ToList();
        }
    }
}