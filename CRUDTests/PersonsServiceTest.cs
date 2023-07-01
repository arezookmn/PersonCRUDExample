using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using FluentAssertions;
using RepositoryContracts;
using System.Linq.Expressions;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personService;
        private readonly ICountriesService _countriesService;
        private readonly IPersonsRepository _personsRepository;
        private readonly Mock<IPersonsRepository> _personRepositoryMock;
        private readonly IFixture _fixture;

        public PersonsServiceTest()
        {
            List<Person> intitialPersonData = new List<Person>();
            List<Country> intitialCountryData = new List<Country>();

            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options
                );

            dbContextMock.CreateDbSetMock<Country>(t => t.Countries, intitialCountryData);
            dbContextMock.CreateDbSetMock<Person>(t => t.Persons, intitialPersonData);

            ApplicationDbContext dbContext = dbContextMock.Object;
            _personRepositoryMock = new Mock<IPersonsRepository>();

            _personsRepository = _personRepositoryMock.Object;

            _personService = new PersonsService(_personsRepository);
            _fixture = new Fixture();
        }
        #region AddPerson

        //when we supply null value as PersonAddRequest,
        //it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;



            //Assert
            Func<Task> action = async() => await _personService.AddPerson(personAddRequest);

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //when we supply null value as PersonName,
        //it should throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
             .With(temp => temp.PersonName, null as string)
             .Create();

            Person person = personAddRequest.ToPerson();

            //When PersonsRepository.AddPerson is called, it has to return the same "person" object
            _personRepositoryMock
             .Setup(temp => temp.AddPerson(It.IsAny<Person>()))
             .ReturnsAsync(person);

            //Act
            Func<Task> action = async () =>
            {
                await _personService.AddPerson(personAddRequest);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }


        //when we supply proper person details,
        //it should insert Person into persons list ,
        //and it should return an object of PersonResponse, which includes with the newly 
        //Generated personId
        [Fact]
        public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
             .With(temp => temp.Email, "someone@example.com")
             .Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse person_response_expected = person.ToPersonResponse();

            //If we supply any argument value to the AddPerson method, it should return the same return value
            _personRepositoryMock.Setup
             (temp => temp.AddPerson(It.IsAny<Person>()))
             .ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_add = await _personService.AddPerson(personAddRequest);

            person_response_expected.PersonId = person_response_from_add.PersonId;

            //Assert
            person_response_from_add.PersonId.Should().NotBe(Guid.Empty);
            person_response_from_add.Should().Be(person_response_expected);
        }


        #endregion

        #region GetPersonByPersonId

        [Fact]
        public async Task GetPersonByPersonId_NullPersonId_ToBeNul()
        {
            //Arrange
            Guid? guid = null;
            //Acts
            PersonResponse? response = await _personService.GetPersonByPersonID(guid);
            //Assert
            response.Should().BeNull(); 
        }

        //if we supply a valid personId it should return
        //the valid person details as Response object 
        [Fact]
        public async Task GetPersonByPersonID_WithPersonID_ToBeSucessful()
        {
            //Arange
            Person person = _fixture.Build<Person>()
             .With(temp => temp.Email, "email@sample.com")
             .With(temp => temp.Country, null as Country)
             .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
             .ReturnsAsync(person);

            //Act
            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(person.PersonId);

            //Assert
            person_response_from_get.Should().Be(person_response_expected);
        }
        #endregion


        #region GetAllPersons
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            //Arrange 
            IEnumerable<Person> persons = new List<Person>();
            _personRepositoryMock.Setup(t => t.GetAllPersons()).ReturnsAsync(persons);
            //Acts
            List<PersonResponse> personResponses = await _personService.GetAllPersons();
            //Assert
            personResponses.Should().BeEmpty(); 
        }

        [Fact]
        public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
        {
            //Arrange

            Person person1 = _fixture.Build<Person>()
                    .With(temp => temp.Country, null as Country)
                    .With(t => t.Email, "someone@gmail.com").Create();

            Person person2 = _fixture.Build<Person>()
                    .With(temp => temp.Country, null as Country)
                    .With(t => t.Email, "someone@gmail.com").Create();

            Person person3 = _fixture.Build<Person>()
                    .With(temp => temp.Country, null as Country)
                    .With(t => t.Email, "someone@gmail.com").Create();

            List<Person> persons = new List<Person>() { person1, person2, person3};

            _personRepositoryMock.Setup( t => t.GetAllPersons()).ReturnsAsync(persons);

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            //Acts
            List<PersonResponse> personResponse_list_fromGet = await _personService.GetAllPersons();

            personResponse_list_fromGet.Should().BeEquivalentTo(person_response_list_expected);

        }
        #endregion


        #region GetFilterdPerson
        //if the search text is empty and search by is "PersonName", it should return all person
        [Fact]
        public async void GetFilterdPerson_EmptySearchText_ToBeSuccessful() 
        {
            //Arrange

            Person person1 = _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(t => t.Email, "someone@gmail.com")
                .Create();

            Person person2 = _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(t => t.Email, "someone@gmail.com")
                .Create();

            Person person3 = _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(t => t.Email, "someone@gmail.com")
                .Create();


            List<Person> persons = new List<Person>() { person1, person2, person3 };

            IEnumerable<PersonResponse> personResponse_FromAdd= persons.Select(t => t.ToPersonResponse());

            _personRepositoryMock.Setup(t => t.GetFilteredPersons
            (It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);


            //Acts
            List<PersonResponse> personResponse_list_fromGet = await _personService.
                GetFilteredPerson(nameof(Person.PersonName), string.Empty);


            personResponse_list_fromGet.Should().BeEquivalentTo(personResponse_FromAdd);

        }



        //search based on person name with some search string,
        //it should return the matching persons
        [Fact]
        public async Task GetFilterdPerson_SearchByPersonName_ToBeSuccessful()
        {

            Person person1 = _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(t => t.Email, "someone@gmail.com")
                .Create();

            Person person2 = _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(t => t.Email, "someone@gmail.com")
                .Create();

            Person person3 = _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(t => t.Email, "someone@gmail.com")
                .Create();


            List<Person> persons = new List<Person>() { person1, person2, person3 };

            IEnumerable<PersonResponse> personResponse_FromAdd = persons.Select(t => t.ToPersonResponse());

            _personRepositoryMock.Setup(t => t.GetFilteredPersons
            (It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);


            //Acts
            List<PersonResponse> personResponse_list_fromGet = await _personService.
                GetFilteredPerson(nameof(Person.PersonName), "sa");


            personResponse_list_fromGet.Should().BeEquivalentTo(personResponse_FromAdd);

        }


        #endregion

        #region GetSortedPersons

        //When we sort based on PersonName in DESC, it should return persons list in descending on PersonName
        [Fact]
        public async Task GetSortedPersons_ToBeSuccessful()
        {
            //Arrange

            Person person1 = _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(t => t.Email, "someone@gmail.com")
                .Create();

            Person person2 = _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(t => t.Email, "someone@gmail.com")
                .Create();

            Person person3 = _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(t => t.Email, "someone@gmail.com")
                .Create();


            List<Person> persons = new List<Person>() { person1, person2, person3 };

            List<PersonResponse> person_response_list_from_add = persons.Select(t => t.ToPersonResponse()).ToList();

            _personRepositoryMock.Setup(t => t.GetAllPersons())
                .ReturnsAsync(persons);

            List<PersonResponse> allPersons = await _personService.GetAllPersons();

            //Act
            List<PersonResponse> persons_list_from_sort = await _personService
                .GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);


            person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();

            persons_list_from_sort.Should().BeInDescendingOrder(t => t.PersonName);
        }
        #endregion


        #region UpdatedPerson
        //When we supplay null as personUpdateRequest, it should throw ArgumentNullExeption
        [Fact]
        public async Task UpdatedPerson_NullPerson_ToBeArgumentNullException()
        {
            //Arrange 
            PersonUpdateRequest? personUpdate = null;

            //Assert
             Func<Task> action = async() =>
            {
                //Acts
                await _personService.UpdatePerson(personUpdate);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }
        //When we supplay invalid personUpdateRequest, it should throw ArgumentExeption
        [Fact]
        public async Task UpdatedPerson_InvalidPersonID_ToBeArgumentException()
        {
            //Arrange 
            PersonUpdateRequest? personUpdate =  _fixture.Build<PersonUpdateRequest>()
                .Create();

            //Assert
            Func<Task> action = async() =>
            {
                //Acts
                await _personService.UpdatePerson(personUpdate);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }


        [Fact]
        //When person Name is null it should throw argument exeption
        public async Task UpdatedPerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
             .With(temp => temp.PersonName, null as string)
             .With(temp => temp.Email, "someone@example.com")
             .With(temp => temp.Country, null as Country)
             .With(temp => temp.Gender, "Male")
             .Create();

            PersonResponse person_response_from_add = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();


            //Act
            var action = async () =>
            {
                await _personService.UpdatePerson(person_update_request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }



        [Fact]
        //First add a new person and try to update the person name and email
        public async Task UpdatedPerson_PersonFullDetails_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
             .With(temp => temp.Email, "someone@example.com")
             .With(temp => temp.Country, null as Country)
             .With(temp => temp.Gender, "Male")
             .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response_expected.ToPersonUpdateRequest();

            _personRepositoryMock
             .Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
             .ReturnsAsync(person);

            _personRepositoryMock
             .Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
             .ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_update = await _personService.UpdatePerson(person_update_request);

            //Assert
            person_response_from_update.PersonId.Should().Be
                (person_response_expected.PersonId);
        }

        #endregion

        #region DeletePerson
        //if you supplay an valid person id ,it should return true
        [Fact]
        public async Task DeletePerson_ValidPersonID_ToBeSuccessful()
        {
            //Arrange

            Person person = _fixture.Build<Person>()
                .With(temp => temp.Country, null as Country)
                .With(t => t.Email, "someone@gmail.com").Create();

            PersonResponse response_fromAdd = person.ToPersonResponse();

            _personRepositoryMock.Setup(t => t.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            _personRepositoryMock.Setup(t => t.DeletePersonByPersonID(It.IsAny<Guid>()))
               .ReturnsAsync(true);

            //Acts
            bool isValid =await _personService.DeletePerson(response_fromAdd.PersonId);

            //Assert
            isValid.Should().BeTrue();
        }

        //if you supplay an invalid person id ,it should return false
        [Fact]
        public async Task DeletePerson_InValidPersonID()
        {
            //Arrange

            //Acts
            bool isValid = await _personService.DeletePerson(Guid.NewGuid());

            //Assert
            isValid.Should().BeFalse();
        }
        #endregion
    }




}
