using System.Collections.Generic;
using VismaShortageManagement.Models;

namespace VismaShortageManagement.Services
{
    public interface IDataService
    {
        List<Shortage> LoadShortages();
        void SaveShortages(List<Shortage> shortages);
    }
}