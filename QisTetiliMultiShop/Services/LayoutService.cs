using Microsoft.EntityFrameworkCore;
using QisTetiliMultiShop.DAL;
using QisTetiliMultiShop.Models;

namespace QisTetiliMultiShop.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _db;

        public LayoutService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Tuple<List<Category>,Dictionary<string,string>>> GetSettingsAsync()
        {
            return Tuple.Create(await _db.Categories.ToListAsync(),await _db.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value));

        }
    }
}
