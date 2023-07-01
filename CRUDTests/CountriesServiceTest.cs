using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly ICountriesRepository _countriesRepository;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly IFixture _fixture;

        public CountriesServiceTest()
        {
            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;
            _countriesService = new CountriesService(_countriesRepository);
            _fixture = new Fixture();

        }

        #region AddCountry
        //When CountryAddRequest is null , it should throw ArgumentNullExeption
        [Fact]
        public async Task AddCountry_NullCountry_ToBeArgumentNullException()
        {
            //Arrange
            CountryAddRequest? request = null;


            //Acts
            var action = async () => await _countriesService.AddCountry(request);
            await action.Should().ThrowAsync<ArgumentNullException>();
        }


        //Whew the CountryName is null it has throw ArgumentExeption
        [Fact]
        public async Task AddCountry_CountryNameIsNull_ToBeArgumentException()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
             .With(temp => temp.CountryName, null as string)
             .Create();

            Country country = _fixture.Build<Country>()
                 .With(temp => temp.Persons, null as List<Person>).Create();

            _countriesRepositoryMock.Setup(t => t.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);

            _countriesRepositoryMock.Setup(t => t.GetCountryByCountryID(It.IsAny<Guid>()))
                .ReturnsAsync(country);

            //Act
            var action = async () => await _countriesService.AddCountry(request);

            await action.Should().ThrowAsync<ArgumentException>();
        }


        //When the CountryName is duplicate, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName_ToBeArgumentException()
        {
            //Arrange
            CountryAddRequest first_country_request = _fixture.Build<CountryAddRequest>()
                 .With(temp => temp.CountryName, "Test name").Create();
            CountryAddRequest second_country_request = _fixture.Build<CountryAddRequest>()
              .With(temp => temp.CountryName, "Test name").Create();

            Country first_country = first_country_request.ToCountry();
            Country second_country = second_country_request.ToCountry();

            _countriesRepositoryMock
             .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
             .ReturnsAsync(first_country);

            //Return null when GetCountryByCountryName is called
            _countriesRepositoryMock
             .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
             .ReturnsAsync(null as Country);

            CountryResponse first_country_from_add_country = await _countriesService.AddCountry(first_country_request);

            //Act
            var action = async () =>
            {
                //Return first country when GetCountryByCountryName is called
                _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(first_country);

                _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(first_country);

                await _countriesService.AddCountry(second_country_request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }



        //When you supply proper country Details it should insert Country to list
        [Fact]
        public async Task AddCountry_FullCountry_ToBeSuccessful()
        {
            //Arrange
            CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();
            Country country = country_request.ToCountry();
            CountryResponse country_response = country.ToCountryResponse();

            _countriesRepositoryMock
             .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
             .ReturnsAsync(country);

            _countriesRepositoryMock
             .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
             .ReturnsAsync(null as Country);


            //Act
            CountryResponse country_from_add_country = await _countriesService.AddCountry(country_request);

            country.CountryId = country_from_add_country.CountryId;
            country_response.CountryId = country_from_add_country.CountryId;

            //Assert
            country_from_add_country.CountryId.Should().NotBe(Guid.Empty);
            country_from_add_country.CountryId.Should().Be(country_response.CountryId);
        }

        #endregion

        #region GetAllCountries
        [Fact]
        //the list of country should be empty by default (before adding any countries)
        public async Task GetAllCountries_EmptyList()
        {
            _countriesRepositoryMock.Setup(t => t.GetAllCountries()).ReturnsAsync(new List<Country>());
            //Acts
            List<CountryResponse> countryResponses =
                await _countriesService.GetAllCountries();
            //Assert
            countryResponses.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            //Arrange
            List<Country> country_list = new List<Country>() {
                _fixture.Build<Country>()
                .With(temp => temp.Persons, null as List<Person>).Create(),
                _fixture.Build<Country>()
                .With(temp => temp.Persons, null as List<Person>).Create()
              };

            List<CountryResponse> countryResponseList_fromAdd = country_list.Select(t => t.ToCountryResponse()).ToList();

            _countriesRepositoryMock.Setup(t => t.GetAllCountries()).ReturnsAsync(country_list);

            List<CountryResponse> actualResponses = await _countriesService.GetAllCountries();

            actualResponses[0].CountryId.Should().Be(countryResponseList_fromAdd[0].CountryId);
            actualResponses[0].CountryName.Should().Be(countryResponseList_fromAdd[0].CountryName);

        }
        #endregion

        #region GetCountryByCountryId
        [Fact]
        public async Task GetCountryByCountryId_NullCountryId()
        {
            //Arrange
            Guid? countryID = null;

            _countriesRepositoryMock
             .Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>()))
             .ReturnsAsync(null as Country);

            //Act
            CountryResponse? country_response_from_get_method = await _countriesService.GetCountryById(countryID);


            //Assert
            country_response_from_get_method.Should().BeNull();
        }

        [Fact]
        //If we supply a valid country id, it should return the matching country details as CountryResponse object
        public async Task GetCountryByCountryID_ValidCountryID_ToBeSuccessful()
        {
            //Arrange
            Country country = _fixture.Build<Country>()
              .With(temp => temp.Persons, null as List<Person>)
              .Create();
            CountryResponse country_response = country.ToCountryResponse();

            _countriesRepositoryMock
             .Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>()))
             .ReturnsAsync(country);

            //Act
            CountryResponse? country_response_from_get = await _countriesService.GetCountryById(country.CountryId);


            //Assert
            country_response_from_get.Should().BeEquivalentTo(country_response);

        }
        #endregion
    }
}

