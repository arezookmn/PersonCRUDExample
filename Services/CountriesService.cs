using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService

    {   private readonly ICountriesRepository _countriesRepository;
        public CountriesService(ICountriesRepository countriesRepository) 
        {
            _countriesRepository = countriesRepository;

        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            //Validation:CountryAddRequest cant be null
            if(countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            //Validation:CountryName cant be Null
            if(countryAddRequest.CountryName== null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }
            //validation: CountyName cant be duplicated
            if(await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null)
            {
                throw new ArgumentException();
            }

            //convert object from CountryAddRequest to Country
            Country country = countryAddRequest.ToCountry();

            //generate countryId
            Guid countryId = Guid.NewGuid();
            country.CountryId = countryId;
            //Add to countryList
            _countriesRepository.AddCountry(country);
            //return CountryResponse
            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return (await _countriesRepository.GetAllCountries())
                .Select(country => country.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse>? GetCountryById(Guid? id)
        {
            //check if CountryId is not Null
            if(id == null)
            {
                return null;
            }
            //Get matching country from list<Country> based CountryId
            Country? response = await _countriesRepository.GetCountryByCountryID(id.Value);

            if(response == null) return null;

            return response.ToCountryResponse() ;

        }
    }
}