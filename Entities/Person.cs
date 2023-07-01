using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// Person Domain model class 
    /// </summary>
    public class Person
    {
        [Required(ErrorMessage = "Person name cant be blank.")]
        [Key]
        public Guid PersonId { get; set; }
        [StringLength(40)]
        [Required(ErrorMessage = "Email cant be blank.")]
        public string? PersonName { get; set; }
        [StringLength(40)]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set;}
        [StringLength(6)]
        public string? Gender { get; set;}
        //uniqeIdentifier
        public Guid? CountryId { get; set; } //forign key
        [StringLength(200)]
        public string? Address { get; set; }
        //bit 
        public bool ReciveNewsLetters { get; set; }

        public string? TIN { get; set; }

        [ForeignKey("CountryId")]
        public Country? Country { get ; set; }
    }
}
