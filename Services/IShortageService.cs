using System;
using System.Collections.Generic;
using VismaShortageManagement.Models;

namespace VismaShortageManagement.Services
{
    public interface IShortageService
    {
        bool RegisterShortage(Shortage shortage);
        bool DeleteShortage(string title, Room room, string userName, bool isAdmin);
        List<Shortage> GetShortages(string userName, bool isAdmin, string titleFilter = null, 
            DateTime? fromDate = null, DateTime? toDate = null, Category? categoryFilter = null, 
            Room? roomFilter = null);
    }
}