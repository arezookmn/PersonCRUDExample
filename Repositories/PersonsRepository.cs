using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public PersonsRepository(ApplicationDbContext applicationDb)
        {
            _dbContext = applicationDb;
        }
        public async Task<Person> AddPerson(Person person)
        {
            _dbContext.Persons.Add(person);
            await _dbContext.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePersonByPersonID(Guid PersonID)
        {
            _dbContext.Persons.RemoveRange(_dbContext.Persons.Where( p => p.PersonId == PersonID ));
            int rowsDeleted = await _dbContext.SaveChangesAsync();
            return rowsDeleted > 0;
        }

        public async Task<IEnumerable<Person>> GetAllPersons()
        {
            return await _dbContext.Persons.Include("Country").ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            return await _dbContext.Persons.Include("Country")
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<Person?> GetPersonByPersonID(Guid personID)
        {
            return await _dbContext.Persons.Include("Country")
                .Where(p => p.PersonId == personID)
                .FirstOrDefaultAsync();
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? matchingPerson = await _dbContext.Persons.FirstOrDefaultAsync(t => t.PersonId == person.PersonId);
            if (matchingPerson == null)
                return person;
            matchingPerson.PersonName = person.PersonName;
            matchingPerson.Gender = person.Gender;
            matchingPerson.Address = person.Address;
            matchingPerson.ReciveNewsLetters = person.ReciveNewsLetters;
            matchingPerson.CountryId = person.CountryId;
            matchingPerson.Email = person.Email;
            matchingPerson.DateOfBirth = person.DateOfBirth;    

            int countUpdated = await _dbContext.SaveChangesAsync();

            return matchingPerson;

        }
    }
}
