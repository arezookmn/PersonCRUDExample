using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class CountryResponse
    {
        public string? CountryName { get; set; } 
        public Guid CountryId { get; set; }
        //public override bool Equals(object? obj)
        //{
        //    if (obj == null) return false;
        //    if (obj.GetType() != typeof(Country)) { return false; }
        //    CountryResponse countryResponse = (CountryResponse)obj;
        //    return this.CountryId == countryResponse.CountryId 
        //        && this.CountryName == countryResponse.CountryName;
        //}

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public static class CountryExtention
    {
        public static CountryResponse ToCountryResponse(this  Country country)
        {
            return new CountryResponse { CountryName = country.CountryName
                , CountryId= country.CountryId };
        }
    }
}
