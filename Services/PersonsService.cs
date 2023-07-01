using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        private readonly IPersonsRepository _personsRepository;
      //  private readonly ICountriesService _countriesService;

        public PersonsService(IPersonsRepository personsRepository)
        {

            _personsRepository = personsRepository;

        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest == null) throw new ArgumentNullException(nameof(personAddRequest)); //Check personAddRequest is not null
                                                                                                     //if(string.IsNullOrEmpty(personAddRequest.PersonName)) throw new ArgumentException("PersonName cant be blank!"); //Validate all peroprety of personAddRequest

            //Model Validation

            ValidationHelper.ModelValidation(personAddRequest);


            Person person = personAddRequest.ToPerson();//Convert to person
            person.PersonId = Guid.NewGuid();//Genrate a new personId
            //_db.Persons.Add(person);//Add person to db
            //_db.SaveChanges();
           await _personsRepository.AddPerson(person);
            return person.ToPersonResponse();

        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            //List<PersonResponse> responses = new List<PersonResponse>();
            //foreach (Person person in _persons)
            //{
            //    responses.Add( person.ToPersonResponse());
            //}
            var persons = await _personsRepository.GetAllPersons();
            //return responses;

            return persons.Select(temp => temp.ToPersonResponse()).ToList();
        }

        public async Task<PersonResponse>? GetPersonByPersonID(Guid? perosnId)
        {
            if (perosnId == null)
                return null;
            Person? person = await _personsRepository.GetPersonByPersonID(perosnId.Value);
            if (person == null) return null;
            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPerson(string searchBy, string? searchString)
        {
            IEnumerable<Person> persons = searchBy switch
            {
                nameof(PersonResponse.PersonName) =>
                   await _personsRepository.GetFilteredPersons(temp =>
                   temp.PersonName.Contains(searchString)),

                nameof(PersonResponse.Email) =>
                 await _personsRepository.GetFilteredPersons(temp =>
                 temp.Email.Contains(searchString)),


                nameof(PersonResponse.Address) =>
                    await _personsRepository.GetFilteredPersons(temp =>
                    temp.Address.Contains(searchString)),


                nameof(PersonResponse.DateOfBirth) =>
                    await _personsRepository.GetFilteredPersons(temp =>
                    temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString)),


                nameof(PersonResponse.Gender) =>
                    await _personsRepository.GetFilteredPersons(temp =>
                    temp.Gender.Contains(searchString)),


                nameof(PersonResponse.CountryId) =>
                    await _personsRepository.GetFilteredPersons(temp =>
                    temp.Country.CountryName.Contains(searchString)),

                //defult case
                _ => await _personsRepository.GetAllPersons()
            } ; 

            return  persons.Select(temp => temp.ToPersonResponse()).ToList();
        }


        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
                return allPersons;

            List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Age).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReciveNewsLetters), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.ReciveNewsLetters).ToList(),

                (nameof(PersonResponse.ReciveNewsLetters), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.ReciveNewsLetters).ToList(),

                _ => allPersons //defult case
            };

            return sortedPersons;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if(personUpdateRequest == null)//check for null
                throw new ArgumentNullException();
            //Validation
            ValidationHelper.ModelValidation(personUpdateRequest);

            //get matching person object to update
            Person? person =await _personsRepository.GetPersonByPersonID(personUpdateRequest.PersonID);
            if (person == null) throw new ArgumentException();//invalid personId

            //valid person id, update person
            person.PersonName = personUpdateRequest.PersonName;
            person.DateOfBirth= personUpdateRequest.DateOfBirth;
            person.Gender = personUpdateRequest.Gender.ToString();
            person.Email = personUpdateRequest.Email;
            person.CountryId= personUpdateRequest.CountryId;
            person.ReciveNewsLetters = personUpdateRequest.ReciveNewsLetters;
            person.Address = personUpdateRequest.Address;

            await _personsRepository.UpdatePerson(person);//UPDATE


            return person.ToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            if(personID == null) throw new ArgumentNullException();
            Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);
            if (person == null) return false;
            await _personsRepository.DeletePersonByPersonID(personID.Value);
            return true;
        }
    } 
}
