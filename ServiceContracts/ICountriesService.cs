using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating country entity
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Add Country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest"> country object to add</param>
        /// <returns></returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

        Task<List<CountryResponse>> GetAllCountries();
        /// <summary>
        /// Returns country object based on given countryId
        /// </summary>
        /// <param name="id">CountryId (guid) to search </param>
        /// <returns>Matvhing country as countryResponse </returns>
        Task<CountryResponse>? GetCountryById(Guid? id);

    }
}