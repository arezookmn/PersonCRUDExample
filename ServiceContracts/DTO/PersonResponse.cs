using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents DTo class that is used as return type of most method of Persons service
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonId { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryId { get; set; } //forign key
        public string? Country { get; set; }
        public string? Address { get; set; }
        public double? Age { get; set; }

        public bool ReciveNewsLetters { get; set; }
        /// <summary>
        /// Compares the current object data with the parameter object
        /// </summary>
        /// <param name="obj">The PersonResponse object to compare</param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(PersonResponse)) return false;

            PersonResponse person = (PersonResponse)obj;
            //Compare each property of current object with person
            return PersonId == person.PersonId &&
                PersonName == person.PersonName
                && Email == person.Email &&
                DateOfBirth == person.DateOfBirth &&
                CountryId == person.CountryId &&
                Address == person.Address &&
                Age == person.Age &&
                Gender == person.Gender &&
                ReciveNewsLetters == person.ReciveNewsLetters;//if all these match return true

        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"Person Id : {PersonId}," +
                $" Person Name: {PersonName}, Email : {Email}," +
                $" Address : {Address}, CountryId : {CountryId} ";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonID = PersonId,
                PersonName = PersonName,
                Email = Email,
                CountryId = CountryId,
                DateOfBirth = DateOfBirth,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true)
            };
        }

    }

    public static class PersonExtention
    {
        /// <summary>
        /// an extension method to convert an object of Person class into PersonRespnse class
        /// </summary>
        /// <param name="person">Returns the converted Response Person object</param>
        public static PersonResponse ToPersonResponse(this Person person)
        {
            PersonResponse personResponse = new PersonResponse()
            {
                PersonId = person.PersonId,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                CountryId = person.CountryId,
                Address = person.Address,
                Gender = person.Gender,
                ReciveNewsLetters = person.ReciveNewsLetters,
                Age = (person.DateOfBirth != null)? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365) : null,
                Country = person.Country?.CountryName
        };

            return personResponse;
        }
    }
}
