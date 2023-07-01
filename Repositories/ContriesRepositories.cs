using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Repositories
{
    public class ContriesRepositories : ICountriesRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ContriesRepositories(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Country> AddCountry(Country country)
        {
            _dbContext.Countries.Add(country);  
            await _dbContext.SaveChangesAsync();
            return country;
        }

        public async Task<IEnumerable<Country>> GetAllCountries()
        {
           IEnumerable<Country> countries = await _dbContext.Countries.ToListAsync();
            return countries;
        }

        public async Task<Country?> GetCountryByCountryID(Guid countryID)
        {
            Country? country = await _dbContext.Countries.FirstOrDefaultAsync(c => c.CountryId == countryID);
            return country;
        }

        public async Task<Country?> GetCountryByCountryName(string countryName)
        {
            Country? country = await _dbContext.Countries.FirstOrDefaultAsync(c => c.CountryName == countryName);
            return country;
        }
    }
}