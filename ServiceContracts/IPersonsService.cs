using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    /// <summary>
    /// Represents Business logic for manipulating Person entity
    /// </summary>
    public interface IPersonsService
    {
        /// <summary>
        /// Adds a new Person into the list of persons
        /// </summary>
        /// <param name="personAddRequest"></param>
        /// <returns></returns>
        Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);

        /// <summary>
        /// Returns all persons
        /// </summary>
        /// <returns>returns a list of Objects of PersonResponse type</returns>
        Task<List<PersonResponse>> GetAllPersons();


        /// <summary>
        /// Returns Person object based on the given personId
        /// </summary>
        /// <param name="perosnId">PersonId to search</param>
        /// <returns>Matvhing Person object</returns>
        Task<PersonResponse>? GetPersonByPersonID(Guid? perosnId);

        /// <summary>
        /// Returns all person objects that matches with the 
        /// given search field and search string
        /// </summary>
        /// <param name="searchBy">Search field to search</param>
        /// <param name="searchString">Search stirng to search</param>
        /// <returns>Returns all matching person based on the given search field and search string</returns>
        Task<List<PersonResponse>> GetFilteredPerson(string searchBy, string? searchString);

        /// <summary>
        /// Returns sorted list of persons
        /// </summary>
        /// <param name="allPersons">Represents list of persons to sort</param>
        /// <param name="sortBy">Name of the property (key), based on which the persons should be sorted</param>
        /// <param name="sortOrder">ASC or DESC</param>
        /// <returns>Returns sorted persons as PersonResponse list</returns>
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);

        /// <summary>
        /// Update the spesified person details based on the given person Id
        /// </summary>
        /// <param name="personUpdateRequest">Person details to update including person Id</param>
        /// <returns>Returns person response after updation</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);

        /// <summary>
        /// Delete person based on the given person id 
        /// </summary>
        /// <param name="personID">personId to delete </param>
        /// <returns>Returnss true if the deletion is successful, otherwise false</returns>
        Task<bool> DeletePerson(Guid? personID);
    }
}
