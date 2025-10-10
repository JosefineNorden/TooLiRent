using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooLiRent.Models;

namespace TooLiRent.Core.Interfaces
{
    public interface IToolRepository
    {
        Task<IEnumerable<Tool>> GetAllAsync();
        Task<Tool?> GetByIdAsync(int id);
        Task AddAsync(Tool tool);
        Task UpdateAsync(Tool tool);
        Task DeleteAsync(Tool tool);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Tool>> GetAvailableAsync();
        Task<IEnumerable<Tool>> FilterAsync(string? category, string? status, bool? onlyAvailable);
        Task<List<string>> GetDistinctCategoriesAsync();



    }
}
